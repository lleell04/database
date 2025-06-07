<!-- 管理员页面的侧边栏 -->
<template>
    <aside class="sidebar">
      <nav>
        <ul>
          <!-- <li>
            <router-link to="/platform-info" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets_dxy/platform_icon.svg" alt="Icon1" class="sidebar-icon white-icon">
                <span>平台信息</span>
              </div>
            </router-link>
          </li> -->
          <li>
            <router-link to="/merchant-certification" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets_dxy/certification_icon.svg" alt="Icon2" class="sidebar-icon white-icon">
                <span>商家认证</span>
              </div>
            </router-link>
          </li>
          <li>
            <router-link to="/market-management" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets_dxy/market_icon.svg" alt="Icon3" class="sidebar-icon white-icon">
                <span>市集管理</span>
              </div>
            </router-link>
          </li>
          <li>
            <router-link to="/report-management" class="nav-link" active-class="active-link">
              <div class="button-content">
                <img src="@/assets_dxy/report_icon.svg" alt="Icon4" class="sidebar-icon white-icon">
                <span>举报管理</span>
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
    background-color: #444;
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
    background: #444444;
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
    background-color: #555;
  }
  
  .active-link {
    border: 1px solid #fff;
  }
  </style>
  