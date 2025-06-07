import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  
  plugins: [
    vue(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:7262', // 你的后端服务器地址
        changeOrigin: true, // 改变原始主机头为目标URL
        rewrite: (path) => path.replace(/^\/api/, '') // 可选：重写路径
      }
    }
  },
})
