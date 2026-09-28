import 'package:dio/dio.dart';
import 'token_storage.dart';
import 'package:flutter/foundation.dart';
import '../Data/app_config.dart';

class ApiClient {
  final Dio dio = Dio(BaseOptions(
    //baseUrl:'https://bovine-abridge-doodle.ngrok-free.dev',
    //baseUrl:'https://localhost:7212',
    baseUrl: AppConfig.baseUrl,
    connectTimeout: const Duration(seconds: 5),
    receiveTimeout: const Duration(seconds: 3),
    headers: {
      'Content-Type': 'application/json',
    },
  ));

  ApiClient() {
    dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) async {
        final token = await TokenStorage.getToken();
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        return handler.next(options); 
      },
      onError: (DioException e, handler) {
        debugPrint("Error ${e.message}");
        return handler.next(e);
      },
    ));
  }
}