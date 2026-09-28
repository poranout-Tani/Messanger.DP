import 'package:flutter/material.dart';
import 'package:dio/dio.dart';
import '../Data/api_client.dart';
import '../Data/token_storage.dart';
import '../Screen/chat_list_screen.dart';
import '../main.dart';

class AuthScreen extends StatefulWidget {
  const AuthScreen({super.key});

  @override
  State<AuthScreen> createState() => _AuthScreenState();
}

class _AuthScreenState extends State<AuthScreen> {
  final _formKey = GlobalKey<FormState>();
  
  final _usernameController = TextEditingController();
  final _passwordController = TextEditingController();
  final _apiClient = ApiClient();
  
  bool _isLogin = true;

  void _submit() async {
    if (!_formKey.currentState!.validate()) return;

    final username = _usernameController.text.trim();
    final password = _passwordController.text.trim();

    try {
      if (_isLogin) {
        final response = await _apiClient.dio.post('/Login', data: {
          'username': username,
          'password': password,
        });

        if (response.statusCode == 200 && response.data !=null) {
          final data = response.data as Map<String, dynamic>;
          final token = data['token']?.toString() ?? '';
          final userId = data['userId']?.toString() ?? data['userid']?.toString() ?? ''; 

          if (userId.isEmpty) {
            print("Warning:Servers dint send ID");
          }
          
          await TokenStorage.saveAuthData(token, userId);
          print("Login successful");

          if (mounted) {
            Navigator.pushReplacement(
              context,
              MaterialPageRoute(builder: (context) => ChatListScreen(currentUserId: userId, currentUserName: username),),
            );
          }
        }
      } else {
        final response = await _apiClient.dio.post('/register', data: {
          'username': username,
          'password': password,
        });

        if (response.statusCode == 200 || response.statusCode == 201) {
          print("Registration successful. Can login");
          setState(() => _isLogin = true);
        }
      }
    } on DioException catch (e) {
      final errorMessage = e.response?.data['message'] ?? "Error";
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(errorMessage)),
      );
    }
  }

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {  
    return Scaffold(
      appBar: AppBar(title: Text(_isLogin ? 'Login' : 'Registration'),
        actions: [
          ValueListenableBuilder<ThemeMode>(
            valueListenable: themeNotifier,
            builder: (context, currentMode, _) {
              final isDark = currentMode == ThemeMode.dark;
              return IconButton(
                icon: Icon(isDark ? Icons.wb_sunny_rounded : Icons.nights_stay_rounded),
                tooltip: isDark ? 'Light theme' : 'Night theme',
                onPressed: () {
                  themeNotifier.value = isDark ? ThemeMode.light : ThemeMode.dark;
                },
              );
            },
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              TextFormField(
                controller: _usernameController,
                decoration: const InputDecoration(
                  labelText: 'Username',
                  border: OutlineInputBorder(),
                ),
                validator: (val) => val != null && val.trim().length >= 3 
                    ? null 
                    : 'Username must have min 3 symbols',
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _passwordController,
                decoration: const InputDecoration(
                  labelText: 'Password',
                  border: OutlineInputBorder(),
                ),
                obscureText: true,
                validator: (val) => val!.length >= 6 ? null : 'Password must have min 6 symbils',
              ),
              const SizedBox(height: 20),
              SizedBox(
                width: double.infinity,
                height: 50,
                child: ElevatedButton(
                  onPressed: _submit,
                  child: Text(_isLogin ? 'Login' : 'Registration'),
                ),
              ),
              const SizedBox(height: 12),
              TextButton(
                onPressed: () => setState(() => _isLogin = !_isLogin),
                child: Text(_isLogin ? 'Dont have account.Registration' : 'U must have account.Login'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}