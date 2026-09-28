import 'package:flutter/material.dart';

class CreateGroupScreen extends StatefulWidget {
  final dynamic apiClient;

  const CreateGroupScreen({super.key, required this.apiClient});

  @override
  State<CreateGroupScreen> createState() => _CreateGroupScreenState();
}

class _CreateGroupScreenState extends State<CreateGroupScreen> {
  final TextEditingController _groupNameController = TextEditingController();
  
  List<dynamic> _contacts = [];
  final Set<String> _selectedUserIds = {};
  
  bool _isLoadingContacts = true;
  bool _isCreatingGroup = false;

  @override
  void initState() {
    super.initState();
    _fetchContacts(); 
  }

  Future<void> _fetchContacts() async {
    try {
      final response = await widget.apiClient.dio.get('/contacts');
      
      if (response.data != null && response.data is List) {
        setState(() {
          _contacts = response.data;
          _isLoadingContacts = false;
        });
      }
    } catch (e) {
      debugPrint("Ошибка при загрузке контактов: $e");
      if (mounted) {
        setState(() => _isLoadingContacts = false);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text("Не удалось загрузить контакты: $e"), backgroundColor: Colors.red),
        );
      }
    }
  }

  Future<void> _createGroup() async {
    final groupName = _groupNameController.text.trim();

    if (groupName.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Пожалуйста, введите название группы")),
      );
      return;
    }

    if (_selectedUserIds.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("Выберите хотя бы одного участника")),
      );
      return;
    }

    setState(() => _isCreatingGroup = true);

    try {
      final response = await widget.apiClient.dio.post(
        '/Create',
        data: {
          'chatname': groupName,
          'interlocutorId': null,
          'usersId': null,
          'userIds': _selectedUserIds.toList(),
        },
      );

      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Группа успешно создана!"), backgroundColor: Colors.green),
        );
        Navigator.pop(context);
      }
    } catch (e) {
      debugPrint("Ошибка создания группы: $e");
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text("Ошибка при создании группы: $e"), backgroundColor: Colors.red),
        );
      }
    } finally {
      if (mounted) setState(() => _isCreatingGroup = false);
    }
  }

  @override
  void dispose() {
    _groupNameController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('New Group'),
        actions: [
          _isCreatingGroup
              ? const Center(
                  child: Padding(
                    padding: EdgeInsets.only(right: 16.0),
                    child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                  ),
                )
              : IconButton(
                  icon: const Icon(Icons.check, size: 28),
                  onPressed: _createGroup, 
                ),
        ],
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Row(
              children: [
                const Text(
                  "Group name",
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.w500),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: TextField(
                    controller: _groupNameController,
                    decoration: const InputDecoration(
                      hintText: 'Enter group name...',
                      border: UnderlineInputBorder(),
                      isDense: true,
                    ),
                  ),
                ),
              ],
            ),
          ),

          Container(
            width: double.infinity,
            color: Theme.of(context).brightness == Brightness.dark 
                ? Colors.grey[900] 
                : Colors.grey[200],
            padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
            child: const Text(
              "Contacts",
              style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold, color: Colors.blue),
            ),
          ),

          Expanded(
            child: _isLoadingContacts
                ? const Center(child: CircularProgressIndicator())
                : _contacts.isEmpty
                    ? const Center(child: Text("Список контактов пуст"))
                    : ListView.builder(
                        itemCount: _contacts.length,
                        itemBuilder: (context, index) {
                          final contact = _contacts[index];
                          
                          final String userId = contact['id'] ?? contact['userId'] ?? '';
                          final String username = contact['username'] ?? contact['name'] ?? 'Unknown';

                          final isSelected = _selectedUserIds.contains(userId);

                          return CheckboxListTile(
                            controlAffinity: ListTileControlAffinity.trailing,
                            secondary: const CircleAvatar(
                              backgroundColor: Colors.blue,
                              child: Icon(Icons.person, color: Colors.white),
                            ),
                            title: Text(
                              username,
                              style: const TextStyle(fontWeight: FontWeight.w500),
                            ),
                            subtitle: const Text("last seen recently"),
                            value: isSelected,
                            onChanged: (bool? checked) {
                              setState(() {
                                if (checked == true) {
                                  _selectedUserIds.add(userId);
                                } else {
                                  _selectedUserIds.remove(userId);
                                }
                              });
                            },
                          );
                        },
                      ),
          ),
        ],
      ),
    );
  }
}