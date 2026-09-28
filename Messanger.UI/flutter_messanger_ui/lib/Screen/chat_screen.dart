import 'package:flutter/material.dart';
import 'package:flutter_chat_core/flutter_chat_core.dart' as core;
import 'package:dio/dio.dart';
import '../Data/api_client.dart';
import 'dart:async';

class ChatScreen extends StatefulWidget {
  final String currentUserId;
  final String chatId;
  final String chatName;

  const ChatScreen({
    super.key,
    required this.currentUserId,
    required this.chatId,
    required this.chatName,
  });

  @override
  State<ChatScreen> createState() => _ChatScreenState();
}

class _ChatScreenState extends State<ChatScreen> {
  final _apiClient = ApiClient();
  bool _isLoading = true;
  Timer? _refreshTime;
  final Set<String> _loadedMessageIds = {};
  final Map<String, String> _userName = {};
  final List<core.TextMessage> _messages = [];
  final TextEditingController _textController = TextEditingController();
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _loadMessages();
  }

  @override
  void dispose() {
    _refreshTime?.cancel();
    _textController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  Future<String> _resolveUsername(String userId) async {
    if (userId == widget.currentUserId) return 'Вы';
    if (_userName.containsKey(userId)) return _userName[userId]!;

    try {
      final response = await _apiClient.dio.get('/profile/$userId');
      if (response.statusCode == 200 && response.data != null) {
        final name = response.data['username']?.toString() ?? userId;
        _userName[userId] = name;
        return name;
      }
    } on DioException catch (e) {
      debugPrint("Resolve user error: ${e.message}");
    }

    final shortId = userId.length > 8 ? userId.substring(0, 8) : userId;
    _userName[userId] = shortId;
    return shortId;
  }

  core.TextMessage _parseMessage(dynamic item) {
    final authorId = item['senderId']?.toString() ??
                     item['authorId']?.toString() ?? '';
    return core.TextMessage(
      id: item['id']?.toString() ??
          DateTime.now().microsecondsSinceEpoch.toString(),
      authorId: authorId,
      text: item['textMessage'] ?? item['text'] ?? '',
      createdAt: item['createdAt'] != null
          ? DateTime.parse(item['createdAt']).toUtc()
          : DateTime.now().toUtc(),
    );
  }

  void _scrollToBottom() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scrollController.hasClients) {
        _scrollController.animateTo(
          _scrollController.position.maxScrollExtent,
          duration: const Duration(milliseconds: 200),
          curve: Curves.easeOut,
        );
      }
    });
  }

  void _loadMessages() async {
    try {
      final response =
          await _apiClient.dio.get('/messages/${widget.chatId}');

      if (response.statusCode == 200 && response.data != null) {
        final List<dynamic> data = response.data;

        final uniqueIds = data
            .map((item) => item['senderId']?.toString() ?? '')
            .where((id) => id.isNotEmpty)
            .toSet();
        for (final userId in uniqueIds) {
          await _resolveUsername(userId);
        }

        final List<core.TextMessage> loaded = [];
        for (final item in data) {
          final msg = _parseMessage(item);
          _loadedMessageIds.add(msg.id);
          loaded.add(msg);
        }

        setState(() {
          _messages.addAll(loaded);
          _isLoading = false;
        });
        _scrollToBottom();
      }
    } on DioException catch (e) {
      debugPrint("Error ${e.message}");
      setState(() => _isLoading = false);
    } finally {
      _refreshTime = Timer.periodic(
        const Duration(milliseconds: 500),
        (_) => _pollNewMessages(),
      );
    }
  }

  void _pollNewMessages() async {
    try {
      final response =
          await _apiClient.dio.get('/messages/${widget.chatId}');

      if (response.statusCode == 200 && response.data != null) {
        final List<dynamic> data = response.data;
        bool hasNew = false;
        for (final item in data) {
          final msg = _parseMessage(item);
          if (!_loadedMessageIds.contains(msg.id)) {
            if (msg.authorId.isNotEmpty) {
              await _resolveUsername(msg.authorId);
            }
            _loadedMessageIds.add(msg.id);
            _messages.add(msg);
            hasNew = true;
          }
        }
        if (hasNew && mounted) {
          setState(() {});
          _scrollToBottom();
        }
      }
    } on DioException catch (e) {
      debugPrint("Poll error ${e.message}");
    }
  }

  void _handleSend() async {
    final text = _textController.text.trim();
    if (text.isEmpty) return;
    _textController.clear();

    try {
      await _apiClient.dio.post(
        '/messages/${widget.chatId}',
        data: {'textMessage': text},
      );
    } on DioException catch (e) {
      debugPrint("Error ${e.message}");
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text("Ошибка отправки: ${e.message}"),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Widget _buildBubble(core.TextMessage msg) {
    final isMe = msg.authorId == widget.currentUserId;
    final isDark = Theme.of(context).brightness == Brightness.dark;
    final senderName = _userName[msg.authorId] ?? msg.authorId;

    final time = msg.createdAt != null
        ? '${msg.createdAt!.toLocal().hour.toString().padLeft(2, '0')}:'
          '${msg.createdAt!.toLocal().minute.toString().padLeft(2, '0')}'
        : '';

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 3),
      child: Column(
        crossAxisAlignment:
            isMe ? CrossAxisAlignment.end : CrossAxisAlignment.start,
        children: [
          // имя отправителя над пузырьком (только для чужих)
          if (!isMe)
            Padding(
              padding: const EdgeInsets.only(left: 4, bottom: 2),
              child: Text(
                senderName,
                style: const TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.bold,
                  color: Colors.blueAccent,
                ),
              ),
            ),
          Container(
            constraints: BoxConstraints(
              maxWidth: MediaQuery.of(context).size.width * 0.7,
            ),
            padding: const EdgeInsets.symmetric(
                horizontal: 14, vertical: 8),
            decoration: BoxDecoration(
              color: isMe
                  ? Colors.blueAccent
                  : (isDark
                      ? const Color(0xFF243447)
                      : Colors.grey[200]),
              borderRadius: BorderRadius.only(
                topLeft: const Radius.circular(16),
                topRight: const Radius.circular(16),
                bottomLeft: Radius.circular(isMe ? 16 : 4),
                bottomRight: Radius.circular(isMe ? 4 : 16),
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                Text(
                  msg.text,
                  style: TextStyle(
                    fontSize: 15,
                    color: isMe
                        ? Colors.white
                        : (isDark ? Colors.white : Colors.black87),
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  time,
                  style: TextStyle(
                    fontSize: 11,
                    color: isMe ? Colors.white70 : Colors.grey,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;

    return Scaffold(
      appBar: AppBar(
        title: Text(
          widget.chatName,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : Column(
              children: [
                // список сообщений
                Expanded(
                  child: _messages.isEmpty
                      ? const Center(
                          child: Text(
                            'Нет сообщений',
                            style: TextStyle(color: Colors.grey),
                          ),
                        )
                      : ListView.builder(
                          controller: _scrollController,
                          padding: const EdgeInsets.symmetric(vertical: 8),
                          itemCount: _messages.length,
                          itemBuilder: (_, i) => _buildBubble(_messages[i]),
                        ),
                ),
                // поле ввода
                Container(
                  padding: const EdgeInsets.symmetric(
                      horizontal: 8, vertical: 8),
                  decoration: BoxDecoration(
                    color: isDark
                        ? const Color(0xFF101921)
                        : Colors.white,
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.08),
                        blurRadius: 4,
                        offset: const Offset(0, -2),
                      )
                    ],
                  ),
                  child: Row(
                    children: [
                      Expanded(
                        child: Container(
                          decoration: BoxDecoration(
                            color: isDark
                                ? const Color(0xFF243447)
                                : Colors.grey[100],
                            borderRadius: BorderRadius.circular(24),
                          ),
                          child: TextField(
                            controller: _textController,
                            minLines: 1,
                            maxLines: 4,
                            decoration: const InputDecoration(
                              hintText: 'Type a message',
                              hintStyle:
                                  TextStyle(color: Colors.grey),
                              border: InputBorder.none,
                              contentPadding: EdgeInsets.symmetric(
                                  horizontal: 16, vertical: 10),
                            ),
                            onSubmitted: (_) => _handleSend(),
                          ),
                        ),
                      ),
                      const SizedBox(width: 8),
                      GestureDetector(
                        onTap: _handleSend,
                        child: Container(
                          width: 44,
                          height: 44,
                          decoration: const BoxDecoration(
                            color: Colors.blueAccent,
                            shape: BoxShape.circle,
                          ),
                          child: const Icon(
                            Icons.send,
                            color: Colors.white,
                            size: 20,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
    );
  }
}