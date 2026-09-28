import 'package:flutter/material.dart';

class SettingsScreen extends StatefulWidget {
  final dynamic apiClient;
  const SettingsScreen({super.key, required this.apiClient});

  @override
  State<SettingsScreen> createState() => _SettingsScreenState();
}



class _SettingsScreenState extends State<SettingsScreen> {
  bool _isUpdatingPassword = false;
  Future<void> _changePassword(String newPassword) async {
    setState(() => _isUpdatingPassword = true);
    try {
      final response = await widget.apiClient.dio.put('/Updatepassword', data: {'newPassword': newPassword.trim()});
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Password changed"), backgroundColor: Colors.green),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text("Error"), backgroundColor: Colors.red),
        );
      }
    } finally {
      if (mounted) setState(() => _isUpdatingPassword = false);
    }
  }

  void _showPasswordDialog() {
    final controller = TextEditingController();
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text("Password change"),
        content: TextField(
          controller: controller,
          obscureText: true,
          decoration: const InputDecoration(labelText: "New Password"),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.pop(context), child: const Text("Cancel")),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              _changePassword(controller.text);
            },
            child: const Text("Save"),
          ),
        ],
      ),
    );
  }

  void _showAboutDialog() {
    showAboutDialog(
      context: context,
      applicationName: "Messenger",
      applicationVersion: "1",
      applicationIcon: const Icon(Icons.chat, size: 40, color: Colors.blue),
      children: [
        const SizedBox(height: 10),
        const Text("Разработчик: Сыдыкжанов Таниржан \nБэкенд: ASP.NET Core\nФронтенд: Flutter"),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Settings")),
      body: ListView(
        children: [
          ListTile(
            leading: const Icon(Icons.lock),
            title: const Text("Change Password"),
            trailing: _isUpdatingPassword 
                ? const SizedBox(width: 20, height: 20, child: CircularProgressIndicator(strokeWidth: 2))
                : const Icon(Icons.arrow_forward_ios, size: 16),
            onTap: _showPasswordDialog,
          ),
          const Divider(),
          ListTile(
            leading: const Icon(Icons.info),
            title: const Text("About the creator"),
            trailing: const Icon(Icons.arrow_forward_ios, size: 16),
            onTap: _showAboutDialog,
          ),
        ],
      ),
    );
  }
}