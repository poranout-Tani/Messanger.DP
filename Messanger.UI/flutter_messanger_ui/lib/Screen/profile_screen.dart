import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import 'dart:io';
import 'package:file_picker/file_picker.dart';
import 'package:pro_image_editor/pro_image_editor.dart';
import 'dart:typed_data';
import 'package:dio/dio.dart'; 
import '../Data/api_client.dart';

class ProfileDialog extends StatefulWidget {
  final String name;
  final String id;
  final String bio;
  final String birthday;
  final String avatar;

  const ProfileDialog({
    super.key,
    required this.name,
    required this.id,
    required this.bio,
    required this.birthday,
    required this.avatar,
  });

  @override
  State<ProfileDialog> createState() => _ProfileDialogState();
}

class _ProfileDialogState extends State<ProfileDialog> {
  late TextEditingController _nameController;
  late TextEditingController _idController;
  late TextEditingController _bioController;
  late TextEditingController _bdayController;
  late String _avatar;
  
  bool _isHovering = false;
  bool _isLoading = false;
  Uint8List? _avatarBytes;

  final _apiClient = ApiClient();

  final _bdayFormatter = MaskTextInputFormatter(
    mask: '##.##.####',
    filter: {"#": RegExp(r'[0-9]')}, 
  );

  Future<void> _pickImage() async {
    FilePickerResult? result = await FilePicker.platform.pickFiles(
      type: FileType.image,
    );

    if (result != null && result.files.single.path != null) {
      final pickedFile = File(result.files.single.path!);
      final ProImageEditorConfigs configs = ProImageEditorConfigs(
        theme: ThemeData.dark().copyWith(
          scaffoldBackgroundColor: Colors.black,
          appBarTheme: const AppBarTheme(
            backgroundColor: Colors.black,
          ),
        ),
        mainEditor: const MainEditorConfigs(
          tools: [
            SubEditorMode.cropRotate, 
            SubEditorMode.paint,      
          ],
        ),
        cropRotateEditor: const CropRotateEditorConfigs(
          tools: [
          CropRotateTool.rotate,
          ],
        ),
        i18n: const I18n(
          done: 'Set Photo',
          cancel: 'Cancel',
        ),
      );

      final editedImage = await Navigator.push(
        context,
        MaterialPageRoute(
          builder: (context) => ProImageEditor.file(pickedFile, configs: configs,callbacks: ProImageEditorCallbacks(
              onImageEditingComplete: (Uint8List bytes) async {
                Navigator.pop(context, bytes);
              },
              onCloseEditor: (editorMode) { 
              },
            ),
          ),
        ),
      );

      if (editedImage != null) {
        setState(() {
          _avatarBytes = editedImage as Uint8List;
          _isHovering = false;
        });
      }
    }
  }

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.name);
    _idController = TextEditingController(text: widget.id);
    _bioController = TextEditingController(text: widget.bio);
    _bdayController = TextEditingController(text: widget.birthday);
    _avatar = widget.avatar;
  }

  @override
  void dispose() {
    _nameController.dispose();
    _idController.dispose();
    _bioController.dispose();
    _bdayController.dispose();
    super.dispose();
  }

  InputDecoration _inputDecoration(String label, {bool readOnly = false}) {
      return InputDecoration(
        labelText: label,
        labelStyle: TextStyle(color: readOnly ? Colors.grey : Colors.blueAccent),
        filled: true,
        fillColor: Colors.black26,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide.none,
        ),
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      );
    }

  Future<void> _saveProfileChanges() async {
    
    
    if (_bdayController.text.isNotEmpty) {
      final parts = _bdayController.text.split('.');
      
      if (parts.length == 3 && parts[2].length == 4) {
        final day = int.tryParse(parts[0]) ?? 0;
        final month = int.tryParse(parts[1]) ?? 0;
        final year = int.tryParse(parts[2]) ?? 0;

        if (day < 1 || day > 30) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Wrong data(max 30 days)'), backgroundColor: Colors.red),
          );
          return;
        }

        if (month < 1 || month > 12) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Wrong data(max 12 month)'), backgroundColor: Colors.red),
          );
          return;
        }

        if (year < 1900 || year > DateTime.now().year) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Indicate actual birhtday'), backgroundColor: Colors.red),
          );
          return;
        }
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Write full data(DD.MM.YYYY)'), backgroundColor: Colors.red),
        );
        return; 
      }
    }

    setState((){
      _isLoading = true;
    });

    try {
      final apiClient = ApiClient();
      final formData = FormData.fromMap({
        'username': _nameController.text.trim(),
        'bio': _bioController.text.trim(),
        'bday': _bdayController.text.trim(),
        if (_avatarBytes !=null)
          'avatar': MultipartFile.fromBytes(
            _avatarBytes!,
            filename: 'avatar_${widget.id}.png',
          ),
      });
      final response = await apiClient.dio.put(
        '/profile?id=${widget.id}',
        data: formData,
      );
      
      
      if (response.statusCode == 200) {
        if(mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Profile updated'), backgroundColor: Colors.green),
          );
          final serverAvatarUrl = response.data['avatarUrl'] ?? response.data ['avatar'] ?? response.data['AvatarUrl'] ?? '';
          Navigator.pop(context,{
          'username': _nameController.text.trim(),
          'bio': _bioController.text.trim(),
          'bday': _bdayController.text.trim(),
          'avatar': serverAvatarUrl,
          });
        }
      }
    } on DioException catch (e) {
      String errorMessage = "Didnt save changes";

      if (e.response !=null) {
        errorMessage = "Error ${e.response?.statusCode}";
        debugPrint("Details ${e.response?.data}");
      } else {
        errorMessage = "Connection error ${e.message}";
      }
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar( 
          SnackBar(content: Text(errorMessage), backgroundColor: Colors.red),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error $e'), backgroundColor: Colors.red),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
        });
      }
    }

  } 

  @override
  Widget build(BuildContext context) {
    //const String baseUrl = "https://bovine-abridge-doodle.ngrok-free.dev";
    final baseUrl = _apiClient.dio.options.baseUrl;
    final theme = Theme.of(context);
    return Dialog(
      backgroundColor: theme.scaffoldBackgroundColor,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 450, maxHeight: 750),
        child: SingleChildScrollView(
            padding: const EdgeInsets.all(24.0),
            child: Column(
            mainAxisSize: MainAxisSize.min,
              children: [          
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  TextButton(onPressed: () => Navigator.pop(context), child: const Text("Cancel")),
                  const Text("Edit Profile", style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18)),
                  _isLoading ? const Padding(
                    padding: EdgeInsets.symmetric(horizontal: 16.0),
                    child: SizedBox(width: 16, height: 16, child: CircularProgressIndicator(strokeWidth: 2),
                    ),
                  )
                  : TextButton(
                    onPressed: _saveProfileChanges,
                    child: const Text("Save", style: TextStyle(fontWeight: FontWeight.bold)),
                  ),
                ],
              ),

              const SizedBox(height: 20),
              MouseRegion(
                onEnter: (_) => setState(() => _isHovering = true),
                onExit: (_) => setState(() => _isHovering = false),
                cursor: SystemMouseCursors.click,
                child: GestureDetector(
                  onTap: _isLoading ? null : _pickImage,
                  child: CircleAvatar(
                    radius: 50,
                    backgroundColor: theme.primaryColor.withOpacity(0.2),
                    backgroundImage: _avatarBytes != null ? MemoryImage(_avatarBytes!) : _avatar.isNotEmpty ? NetworkImage('$baseUrl$_avatar') : null, //('$baseUrl()$_avatar') : null,
                    child: SizedBox.expand(  
                      child: ClipOval(
                        child: Stack(
                          alignment: Alignment.center,
                          children: [
                            if (_avatarBytes == null && _avatar.isEmpty)
                              const Icon(Icons.camera_alt, size: 50),
                            
                            Positioned.fill(
                              child: AnimatedContainer(
                                duration: const Duration(milliseconds: 200),
                                color: _isHovering 
                                    ? Colors.black.withOpacity(0.3)
                                    : Colors.transparent,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),  
                  ),
                ),
              ),
              const SizedBox(height: 25),
              
              TextField(
                controller: _nameController,
                readOnly: _isLoading,
                decoration: _inputDecoration("Name"),
                style: TextStyle(color: theme.brightness == Brightness.dark ? Colors.white : Colors.black),
              ),
              const SizedBox(height: 15),

              TextField(
                controller: _idController,
                readOnly: true, 
                decoration: _inputDecoration("ID (not editable)", readOnly: true).copyWith(
                  suffixIcon: IconButton(icon: const Icon(Icons.copy, size: 16, color: Colors.grey),
                  onPressed: () {
                    Clipboard.setData(ClipboardData(text: _idController.text));
                    ScaffoldMessenger.of(context).showSnackBar( 
                      const SnackBar(content: Text('ID copied'),
                      duration: Duration(seconds: 2),
                      )
                    );
                  },
                  ),  
                ),
              ),
              const SizedBox(height: 15),

              TextField(
                controller: _bioController,
                readOnly: _isLoading,
                maxLength: 30,
                decoration: const InputDecoration(
                  labelText: "Bio",
                ),
              ),
              const SizedBox(height: 10),

              TextField(
                controller: _bdayController,
                readOnly: _isLoading,
                inputFormatters: [_bdayFormatter],
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(
                  labelText: "Birthday",
                ),
              ),
            ],
          ),
        ),  
      ),
    );
  }
}    
