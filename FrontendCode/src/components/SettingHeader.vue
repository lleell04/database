<!-- 设置页面的头部 -->
<template>
    <header class="header">
      <div class="logo">
        <img src="@/assets/logo1.png" alt="Logo" class="icon"/>
        <span style="font-size: 17px;color: black;">瑕宝阁</span>
      </div>
      <div class="user-info">
        <img src="@/assets_dxy/person_icon.svg" alt="Icon" class="icon">
        <span style="font-size: 17px;color: black;">设置</span>
      </div>
      
    </header>
  </template>
    
    <style scoped>
    .header {
      /* background-color:rgb(245,187,187); */
      /* background-color: rgb(162,88,88); */
      /* background-color:#f7f4ed; */
      /* background-color: rgb(239, 235, 235);  */
      background-color: rgb(237,227,228);
      padding: 10px;
      display: flex;
      height: 70px;
    }
  
    .logo {
      display: flex;
      align-items: center;
    }
  
    .icon {
      height: 30px;
      margin-right: 10px;
    }
    
    .user-info {
      margin-left: auto;
      display: flex;
      align-items: center;
    }
    </style>
    