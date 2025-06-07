<!-- 无用的登录页面 -->
<template>
    <div class="loginbox">
        <div class="box">
            <h2>Login</h2>
            
            <div class="input-box">
                <label>账号</label>
                <input type="text">
            </div>
            <div class="input-box">
                <label>密码</label>
                <input type="password">
            </div>
            <el-radio-group v-model="role" class="custom-radio-group">
                <el-radio :label="1">用户</el-radio>
                <el-radio :label="2">商家</el-radio>
                <el-radio :label="3">管理员</el-radio>
            </el-radio-group>
            <div class="btn-box">
                <a href="javascript:void(0)">忘记密码?</a>
                <div>
                    <button>登录</button>
                    <button>注册</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue'

const role = ref(1)
</script>

<style scoped>
* {
    margin: 0;
    top: 0;
    box-sizing: border-box;
}

.loginbox {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    background: url('../assets/wall.jpg') no-repeat center center;
    background-size: cover;
}

.box {
    width: 350px;
    padding: 20px;
    border-top: 1px solid rgba(255, 255, 255, 0.5);
    border-left: 1px solid rgba(255, 255, 255, 0.5);
    border-bottom: 1px solid rgba(255, 255, 255, 0.3);
    border-right: 1px solid rgba(255, 255, 255, 0.3);
    backdrop-filter: blur(1px);
    background: rgba(50, 50, 50, 0.4);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    border-radius: 6px;
}

.box > h2 {
    color: rgba(255, 255, 255, 0.9);
    margin-bottom: 30px;
}

.custom-radio-group {
    margin-bottom: 20px;
    display: flex;
    justify-content: space-between;
    width: 100%;
    padding: 0 20px;
}

.custom-radio-group .el-radio {
    flex: 1;
    display: flex;
    justify-content: center;
}

.custom-radio-group .el-radio__label {
    color: rgba(255, 255, 255, 0.9) !important;
}

.custom-radio-group .el-radio__input.is-checked + .el-radio__label {
    color: rgba(255, 255, 255, 0.9) !important;
}

.custom-radio-group .el-radio__inner::after {
    background-color: rgba(255, 255, 255, 0.9) !important;
}

.custom-radio-group .el-radio__inner {
    border-color: rgba(255, 255, 255, 0.9) !important;
}

.custom-radio-group .el-radio__input:hover .el-radio__inner {
    border-color: rgba(255, 255, 255, 0.9) !important;
}

.box .input-box {
    display: flex;
    flex-direction: column;
    margin-bottom: 10px;
}

.box .input-box  label {
    font-size: 13px;
    color: rgba(255, 255, 255, 0.9);
    margin-bottom: 5px;
}

.box .input-box input {
    letter-spacing: 1px;
    font-size: 14px;
    width: 250px;
    height: 35px;
    border-radius: 5px;
    border: 1px solid rgba(255, 255, 255, 0.3);
    background: rgba(50, 50, 50, 0.3);
    outline: none;
    padding: 0 12px;
    color: rgba(255, 255, 255, 0.9);
    transition: 0.2s;
}

/* 输入框聚焦效果 */
.box .input-box input:focus {
    border: 1px solid rgba(255, 255, 255, 0.8);
}

.box .btn-box {
    width: 250px;
    display: flex;
    flex-direction: column;
}

.box .btn-box > a {
    outline: none;
    display: block;
    width: 250px;
    text-align: end;
    text-decoration: none;
    font-size: 13px;
    color: rgba(255, 255, 255, 0.8);
}

.box .btn-box > a:hover {
    color: rgba(255, 255, 255, 1);
}

.box .btn-box > div {
    margin-top: 10px;
    display: flex;
    justify-content: center;
    align-items: center;
}

.box .btn-box > div > button {
    outline: none;
    margin-top: 10px;
    display: block;
    font-size: 14px;
    border-radius: 5px;
    transition: 0.2s;
}

.box .btn-box > div > button:nth-of-type(1) {
    width: 120px;
    height: 35px;
    color: rgba(255, 255, 255, 0.9);
    border: 1px solid rgba(00, 96, 96, 0.7);
    background: rgba(00, 96, 96, 0.5);
}

.box .btn-box > div > button:nth-of-type(2) {
    width: 120px;
    height: 35px;
    margin-left: 10px;
    color: rgba(255, 255, 255, 0.9);
    border: 1px solid rgba(00, 96, 96, 0.7);
    background: rgba(00, 96, 96, 0.5);
}

.box .btn-box > div > button:hover {
    border: 1px solid rgba(00, 96, 96);
    background: rgba(00, 96, 96);
}
</style>
