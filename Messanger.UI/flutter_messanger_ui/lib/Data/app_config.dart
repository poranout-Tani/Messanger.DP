import 'dart:convert';
import 'dart:io';
import 'package:flutter/foundation.dart';

class AppConfig {
  static String baseUrl = 'https://localhost:7212'; 

  static Future<void> loadConfig() async {
    try {
      final String exePath = Platform.resolvedExecutable;
      final String exeDir = Directory(exePath).parent.path;
      
      final File configFile = File('$exeDir/config.json');

      if (await configFile.exists()) {
        final String content = await configFile.readAsString();
        final Map<String, dynamic> json = jsonDecode(content);
        
        if (json['baseUrl'] != null && json['baseUrl'].toString().isNotEmpty) {
          baseUrl = json['baseUrl'].toString();
          debugPrint("Адрес сервера успешно загружен из AppConfig: $baseUrl");
        }
      } else {
        debugPrint("Файл config.json не найден рядом с .exe. Используется дефолт: $baseUrl");
      }
    } catch (e) {
      debugPrint("Ошибка чтения конфигурации в AppConfig: $e. Используется дефолт.");
    }
  }
}