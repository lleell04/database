<!-- 设置页面的侧边栏 -->
<template>
    <aside class="sidebar">
      <nav>
        <ul>
          <li>
            <router-link to="/BuyerInformation" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets_dxy/certification_icon.svg" alt="Icon2" class="sidebar-icon white-icon">
                <span>账号信息</span>
              </div>
            </router-link>
          </li>
          <li>
            <router-link to="/BuyerWallet" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets/wy/wallet.png" alt="Icon3" class="sidebar-icon white-icon">
                <span>我的钱包</span>
              </div>
            </router-link>
          </li>
          <li>
            <router-link to="/BuyerPassword" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets/wy/key.png" alt="Icon4" class="sidebar-icon white-icon">
                <span>密码重置</span>
              </div>
            </router-link>
          </li>
          <li>
            <router-link to="/LogoutSetting" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets/wy/logout.png" alt="Icon4" class="sidebar-icon white-icon">
                <span>退出登录</span>
              </div>
            </router-link>
          </li>
          <li>
            <router-link to="/home" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets/wy/homepage.png" alt="Icon4" class="sidebar-icon white-icon">
                <span>返回首页</span>
              </div>
            </router-link>
          </li>
        </ul>
      </nav>
    </aside>
  </template>
  
  <style scoped>
  .sidebar {
    width: 150px;
    /* background-color: rgb(241,164,164); */
    /* background-color: rgb(171,79,79); */
    background-color:#82111f;
    color: #fff;
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 60px 0;
  }
  
  .white-icon {
    filter: brightness(0) invert(1);
  }
  
  .button-content {
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  
  nav ul {
    list-style: none;
    padding: 0;
  }
  
  nav li {
    width: 100%;
    text-align: center;
    margin: 0 0;
  }
  
  .nav-link {
    color: #ffffff;
    /* background-color: rgb(241,164,164); */
    /* background-color: rgb(171,79,79); */
    background-color:#82111f;
    border: none;
    text-decoration: none;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 150px;
    height: 130px;
    padding: 10px 0;
    cursor: pointer;
  }
  
  .nav-link:hover {
    /* background: rgb(238,142,142); */
    /* background-color: rgb(180,69,69); */
    background-color:#6e0e1a;
  }
  
  .active-link {
    border: 1px solid #fff;
  }
  </style>
  