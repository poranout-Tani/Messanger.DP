import 'package:flutter/material.dart';
import 'package:dio/dio.dart';
import 'package:flutter_messanger_ui/Screen/profile_screen.dart';
import '../Data/token_storage.dart';
import '../Data/api_client.dart'; 
import '../Screen/auth_screen.dart';
import '../Screen/chat_screen.dart';
import '../main.dart'; 
import 'contacts_screen.dart';
import '../Screen/settings_screen.dart';
import '../Screen/group_create_screen.dart';
import 'dart:async';

class ChatListScreen extends StatefulWidget {
  final String currentUserId;
  final String currentUserName;
  
  const ChatListScreen({
    super.key, 
    required this.currentUserId, 
    this.currentUserName = "User",
  });

  @override
  State<ChatListScreen> createState() => _ChatListScreenState();
}
// final _apiClient = ApiClient(); 
class _ChatListScreenState extends State<ChatListScreen> {
  final _apiClient = ApiClient(); 
  
  List<dynamic> _chats = [];
  bool _isLoading = true;
  Timer? _refreshTime;

  final TextEditingController _searchController = TextEditingController();
  String _searchQuery = "";

  late String _currentUserName;
  String _bio = "";
  String _bday = "";
  String _avatar = "";


  @override
  void initState() {
    super.initState();
    _currentUserName = widget.currentUserName;
    _fetchChats();
    _fetchUserProfile();
    _refreshTime = Timer.periodic(
      const Duration(milliseconds: 500),
      (_) => _pollChats(),
    );
  }
  
  @override
  void dispose() {
    _refreshTime?.cancel();
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _pollChats() async {
    try {
      final response = await _apiClient.dio.get('/my-chats');
      if (response.statusCode == 200 && mounted) {
        final newChats = response.data as List<dynamic>;
        final Set<String> newIds = newChats
        .map((c) => c['id']?.toString() ?? '')
        .toSet();
        final Set<String> oldIds = _chats
        .map((c) => c['id']?.toString() ?? '')
        .toSet();
        if (!newIds.containsAll(oldIds) || !oldIds.containsAll(newIds)) {
          setState(() => _chats = newChats);
        }
      }
    } on DioException catch (e) {
      debugPrint("Poll chats error: ${e.message}");
    }
  }

  Future<void> _fetchChats() async {
    setState(() => _isLoading = true);
    try {
      final response = await _apiClient.dio.get('/my-chats');
      
      if (response.statusCode == 200) {
        setState(() {
          _chats = response.data as List<dynamic>;
        });
      }
    } on DioException catch (e) {
      debugPrint("Error ${e.message}");
      _showErrorSnackBar("Didnt load chats");
    } finally {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _createChat(String chatName, String interlocutorId) async {
    if (chatName.trim().isEmpty || interlocutorId.trim().isEmpty) {
      _showErrorSnackBar("Write in all fields");
      return;
    }
    try {
      final response = await _apiClient.dio.post(
        '/Create',
        data: {
          'chatname': chatName,
          'interlocutorId': interlocutorId,
          'usersId': widget.currentUserId,
        },
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        debugPrint("Chats created");
        _fetchChats();
      }
    } on DioException catch (e) {
      debugPrint("Error ${e.message}");
      _showErrorSnackBar("Didnt create chat");
    }
  }

  void _showErrorSnackBar(String message) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message), backgroundColor: Colors.red),
    );
  }

  void _showCreateChatDialog() {
    final nameController = TextEditingController();
    final userController = TextEditingController();

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Create new chat'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: nameController,
              decoration: const InputDecoration(
                labelText: 'Name chat',
                hintText: 'Example: chat1',
              ),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: userController,
              decoration: const InputDecoration(
                labelText: 'Companions ID',
                hintText: 'Write ID',
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              _createChat(nameController.text, userController.text);
              Navigator.pop(context);
            },
            child: const Text('Create'),
          ),
        ],
      ),
    );
  }

  Future<void> _fetchUserProfile() async {
        final response = await _apiClient.dio.get('/profile/${widget.currentUserId}');

        if (response.statusCode == 200 && response.data != null) {

          final data = response.data as Map<String, dynamic>;
          setState(() {
            _currentUserName = data['username'] ?? _currentUserName;
            _bio = data['bio'] ?? "";
            _bday = data['bday'] ?? "";
            _avatar = data['avatarUrl'] ?? data['avatarURL'] ?? data['AvatarUrl'] ?? data['avatar'] ??"";
          });
          }
    }

    Future<void> _deleteChat(String chatId) async {
    try {
      final response = await _apiClient.dio.delete('/Delete', queryParameters: {'id': chatId});
      
      if (response.statusCode == 200) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text("Chats deleted"), backgroundColor: Colors.green),
          );
        }
        _fetchChats(); 
      }
    } on DioException catch (e) {
      debugPrint("Error ${e.message}");
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Chats didnt create"), backgroundColor: Colors.red),
        );
      }
    }
  }

  void _showDeleteDialog(String chatId, String chatName) {
  showDialog(
    context: context,
    builder: (BuildContext context) {
      return AlertDialog(
        title: const Text("Delete chat?"),
        content: Text("Are u sure delete this chat'$chatName'?"),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text("Cancel"),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context); 
              _deleteChat(chatId);  
            },
            child: const Text("Delete", style: TextStyle(color: Colors.red)),
          ),
        ],
      );
    },
  );
}

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    final baseUrl = _apiClient.dio.options.baseUrl;

    final filteredChats = _chats.where((chat) {
      final chatName = (chat['chatName'] ?? chat['name'] ?? '').toString().toLowerCase();
      return chatName.contains(_searchQuery.toLowerCase());
    }).toList();

    return Scaffold(
      drawer: Drawer(
        child: Column(
          children: [
            UserAccountsDrawerHeader(
              currentAccountPicture: CircleAvatar(
                backgroundColor: isDark ? const Color(0xFF101921) : Colors.white,
                backgroundImage: _avatar.isNotEmpty
                ? NetworkImage('$baseUrl$_avatar')
                : null,
                child: _avatar.isEmpty 
                    ? const Icon(Icons.person, size: 40, color: Colors.blueAccent)
                    : null,
                    
              ),
              
              accountName: Text(
                _currentUserName,
                style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
              ),
              accountEmail: const Text(
                'Set Emoji Status',
                style: TextStyle(color: Colors.white70),
              ),
              decoration: const BoxDecoration(
                color: Colors.blueAccent,
              ),
            ),
            Expanded(
              child: ListView(
                padding: EdgeInsets.zero,
                children: [
                  ListTile(
                    leading: const Icon(Icons.person_outline),
                    title: const Text('My Profile'),
                    onTap: () async {
                      Navigator.pop (context);
                      await _fetchUserProfile();
                      final Map<String, dynamic>? updatedData = await showDialog<Map<String, dynamic>>(
                        context: context,
                        builder: (context) => ProfileDialog(name: _currentUserName, id: widget.currentUserId, bio: _bio, birthday: _bday, avatar: _avatar,),                      
                      );
                      debugPrint("ОТВЕТ С СЕРВЕРА: $updatedData");
                      if (updatedData != null) {
                        setState(() {
                          _currentUserName = updatedData['username'] ?? _currentUserName;
                          _bio = updatedData['bio'] ?? _bio;
                          _bday = updatedData['bday'] ?? _bday;
                          _avatar = updatedData['avatarUrl'] ?? updatedData['AvatarURL'] ?? updatedData ['AvatarUrl']?? updatedData ['avatar'] ??_avatar;
                        });
                      }
                    },
                  ),
                  ListTile(
                    leading: const Icon(Icons.group_outlined),
                    title: const Text('New Group'),
                    onTap: () {
                      Navigator.pop(context);
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => CreateGroupScreen(apiClient: _apiClient),
                        ),
                      );
                    },
                  ),
                  ListTile(
                    leading: const Icon(Icons.contacts_outlined),
                    title: const Text('Contacts'),
                    onTap: () {
                      Navigator.pop(context);
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => ContactsScreen(
                            currentUserId: widget.currentUserId, 
                          ),
                        ),
                      ).then((_) {
                        _fetchChats(); 
                      });
                    },
                  ),
                  ListTile(
                    leading: const Icon(Icons.settings_outlined),
                    title: const Text('Settings'),
                    onTap: () {
                      Navigator.pop(context);
                      Navigator.push(
                        context,
                        MaterialPageRoute(builder: (context) => SettingsScreen(apiClient: _apiClient),
                        )
                      );
                    },
                  ),
                  const Divider(),
                  ValueListenableBuilder<ThemeMode>(
                    valueListenable: themeNotifier,
                    builder: (context, currentMode, _) {
                      return SwitchListTile(
                        secondary: const Icon(Icons.nights_stay_outlined),
                        title: const Text('Night Mode'),
                        value: currentMode == ThemeMode.dark,
                        onChanged: (bool value) {
                          themeNotifier.value = value ? ThemeMode.dark : ThemeMode.light;
                        },
                      );
                    },
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
      appBar: AppBar(
        title: Container(
          height: 40,
          decoration: BoxDecoration(
            color: isDark ? const Color(0xFF243447) : Colors.grey[200],
            borderRadius: BorderRadius.circular(20),
          ),
          child: TextField(
            controller: _searchController,
            onChanged: (value) {
              setState(() {
                _searchQuery = value;
              });
            },
            decoration: const InputDecoration(
              hintText: 'Search',
              hintStyle: TextStyle(color: Colors.grey),
              border: InputBorder.none,
              prefixIcon: Icon(Icons.search, color: Colors.grey, size: 20),
              contentPadding: EdgeInsets.symmetric(vertical: 8),        
            ),
          ),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: () async {
              await TokenStorage.clear();
              if (mounted) {
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => const AuthScreen()),
                );
              }
            },
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : filteredChats.isEmpty
              ? const Center(
                  child: Text(
                    'U dont have chat. Press + for create chat',
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Colors.grey, fontSize: 16),
                  ),
                )
              : RefreshIndicator(
                  onRefresh: _fetchChats,
                  child: ListView.builder(
                    itemCount: filteredChats.length,
                    itemBuilder: (context, index) {
                      final chat = filteredChats[index];                    
                      final chatId = chat['id']?.toString() ?? 'Didnt have ID';
                      final chatName = chat['chatName']?.toString() ?? chat['name']?.toString() ?? 'Unnamed chat';

                      return ListTile(
                        leading: CircleAvatar(
                          backgroundColor: Colors.blueAccent.withOpacity(0.1),
                          child: const Icon(Icons.chat_bubble_outline, color: Colors.blueAccent),
                        ),
                        title: Text(chatName),
                        subtitle: Text('ID: $chatId', style: const TextStyle(fontSize: 11)),
                        onTap: () {
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (context) => ChatScreen(
                                currentUserId: widget.currentUserId,
                                chatId: chatId,
                                chatName: chatName, 
                              ),
                            ),
                          );
                        },
                        onLongPress: () {
                          final chatId = chat['id']?.toString() ?? '';
                          final chatName = chat['chatName']?.toString() ?? '';
                          if (chatId.isNotEmpty) {
                            _showDeleteDialog(chatId, chatName);
                          }
                        },
                      );
                    },
                  ),
                ),
      floatingActionButton: FloatingActionButton(
        onPressed: _showCreateChatDialog,
        child: const Icon(Icons.add), 
      ),
    );
  }
}