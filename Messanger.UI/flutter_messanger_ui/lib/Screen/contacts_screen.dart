import 'package:flutter/material.dart';
import 'package:dio/dio.dart';
import '../Data/api_client.dart';
import 'chat_screen.dart';
import '../Screen/group_create_screen.dart';

class ContactsScreen extends StatefulWidget {
  final String currentUserId;

  const ContactsScreen({super.key, required this.currentUserId});

  @override
  State<ContactsScreen> createState() => _ContactsScreenState();
}

class _ContactsScreenState extends State<ContactsScreen> {
  final _apiClient = ApiClient();
  List<dynamic> _allUsers = [];
  List<dynamic> _filteredUsers = [];
  bool _isLoading = true;
  
  final TextEditingController _searchController = TextEditingController();
  String _searchQuery = "";

  @override
  void initState() {
    super.initState();
    _fetchUsers();
    _searchController.addListener(_onSearchChanged);
  }

  @override
  void dispose() {
    _searchController.removeListener(_onSearchChanged);
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _fetchUsers() async {
    setState(() => _isLoading = true);
    try {
      final response = await _apiClient.dio.get('/contacts');
      
      if (response.statusCode == 200 && response.data != null) {
        setState(() {
          final List<dynamic> allUsers = response.data as List<dynamic>;
          
          _allUsers = allUsers.where((u) => u['id']?.toString() != widget.currentUserId).toList();
          _filteredUsers = _allUsers;
        });
      }
    } on DioException catch (e) {
      debugPrint("Error ${e.message}");
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Didnt load contacts"), backgroundColor: Colors.red),
        );
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  Future<void> _addNewContact(String targetUserId) async {
    final cleanId = targetUserId.trim();
    if (cleanId.isEmpty) return;

    if (cleanId == widget.currentUserId) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Cant add yourself"), backgroundColor: Colors.orange),
      );
      return;
    }

    try {
      final response = await _apiClient.dio.post('/contacts/$cleanId');

      if (response.statusCode == 200 || response.statusCode == 201) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text("Contacts has added"), backgroundColor: Colors.green),
          );
        }
        _fetchUsers();
      }
    } on DioException catch (e) {
      debugPrint("Error ${e.message}");
      String errorMsg = "Dont add user";
      if (e.response?.statusCode == 404) {
        errorMsg = "Users with this ID dont found";
      }
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(errorMsg), backgroundColor: Colors.red),
        );
      }
    }
  }

  void _showAddContactDialog() {
    final TextEditingController idController = TextEditingController();

    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text('Add contacts'),
          content: TextField(
            controller: idController,
            decoration: const InputDecoration(
              labelText: 'Write ID',
              hintText: 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx',
              border: OutlineInputBorder(),
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(context),
              child: const Text('Cancel'),
            ),
            ElevatedButton(
              onPressed: () {
                final inputId = idController.text;
                Navigator.pop(context);
                _addNewContact(inputId);
              },
              child: const Text('Add'),
            ),
          ],
        );
      },
    );
  }

  void _onSearchChanged() {
    setState(() {
      _searchQuery = _searchController.text.trim().toLowerCase();
      if (_searchQuery.isEmpty) {
        _filteredUsers = _allUsers;
      } else {
        _filteredUsers = _allUsers.where((user) {
          final username = (user['username'] ?? '').toString().toLowerCase();
          return username.contains(_searchQuery);
        }).toList();
      }
    });
  }

  Future<void> _startChatWithUser(Map<String, dynamic> user) async {
    final interlocutorId = user['id']?.toString() ?? '';
    final interlocutorName = user['username']?.toString() ?? 'Unnamed';

    if (interlocutorId.isEmpty) return;

    try {
      final response = await _apiClient.dio.post(
        '/Create',
        data: {
          'chatname': interlocutorName,
          'interlocutorId': interlocutorId,
          'usersId': widget.currentUserId,
        },
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        final chatId = response.data['id']?.toString() ?? '';

        if (mounted && chatId.isNotEmpty) {
          Navigator.pushReplacement(
            context,
            MaterialPageRoute(
              builder: (context) => ChatScreen(
                currentUserId: widget.currentUserId,
                chatId: chatId,
                chatName: interlocutorName,
              ),
            ),
          );
        }
      }
    } on DioException catch (e) {
      debugPrint("Error ${e.message}");
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Cant open chats"), backgroundColor: Colors.red),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;

    return Scaffold(
      appBar: AppBar(
        titleSpacing: 0,
        title: Padding(
          padding: const EdgeInsets.only(right: 16.0),
          child: TextField(
            controller: _searchController,
            style: TextStyle(color: isDark ? Colors.white : Colors.black),
            decoration: InputDecoration(
              hintText: 'Searching...',
              hintStyle: TextStyle(color: isDark ? Colors.white54 : Colors.black54),
              prefixIcon: Icon(Icons.search, color: isDark ? Colors.white54 : Colors.black54),
              border: InputBorder.none,
              focusedBorder: InputBorder.none,
              enabledBorder: InputBorder.none,
            ),
          ),
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : ListView(
              children: [
                _buildActionButton(
                  icon: Icons.group_outlined,
                  title: 'New Group',
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
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                  color: isDark ? const Color(0xFF101921) : Colors.grey[100],
                  child: Text(
                    _searchQuery.isEmpty ? 'Contacts' : 'Search results',
                    style: const TextStyle(
                      color: Colors.blueAccent,
                      fontWeight: FontWeight.bold,
                      fontSize: 13,
                    ),
                  ),
                ),

                if (_filteredUsers.isEmpty)
                  const Padding(
                    padding: EdgeInsets.all(20.0),
                    child: Center(child: Text('List of contacts empty')),
                  )
                else
                  ..._filteredUsers.map((user) {
                    final username = user['username']?.toString() ?? 'Unnamed';
                    final bio = user['bio']?.toString() ?? '';
                    final avatarUrl = user['avatarUrl'] ?? user['avatar'] ?? '';

                    return ListTile(
                      leading: CircleAvatar(
                        radius: 22,
                        backgroundColor: Colors.blueAccent.withOpacity(0.1),
                        backgroundImage: avatarUrl.isNotEmpty
                            ? NetworkImage(avatarUrl.startsWith('http')
                                ? avatarUrl
                                : '${_apiClient.dio.options.baseUrl}$avatarUrl')
                            : null,
                        child: avatarUrl.isEmpty
                            ? const Icon(Icons.person, color: Colors.blueAccent, size: 24)
                            : null,
                      ),
                      title: Text(
                        username,
                        style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
                      ),
                      subtitle: bio.isNotEmpty 
                          ? Text(bio, maxLines: 1, overflow: TextOverflow.ellipsis) 
                          : const Text('last seen recently', style: TextStyle(color: Colors.grey)),
                      onTap: () => _startChatWithUser(user),
                    );
                  }),
              ],
            ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _showAddContactDialog,
        backgroundColor: Colors.blueAccent,
        icon: const Icon(Icons.person_add_alt_1, color: Colors.white),
        label: const Text('Add Contact', style: TextStyle(color: Colors.white)),
      ),
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required String title,
    required VoidCallback onTap,
  }) {
    return ListTile(
      leading: CircleAvatar(
        radius: 20,
        backgroundColor: Colors.transparent,
        child: Icon(icon, color: Colors.blueAccent, size: 26),
      ),
      title: Text(
        title,
        style: const TextStyle(fontWeight: FontWeight.w500, fontSize: 15),
      ),
      onTap: onTap,
    );
  }
}