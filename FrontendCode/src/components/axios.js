import axios from 'axios';


const axiosInstance = axios.create({
  baseURL: 'https://localhost:7262/api',
  // baseURL: 'http://localhost:5262/api', 
  //baseURL: 'http://47.97.5.21:5173/api',
  withCredentials: true, // 允许跨域请求发送 Cookie
  timeout: 60000, // 请求超时时间
  headers: {
    'Content-Type': 'application/json',
  },

}); 

export default axiosInstance;