<template>
    <div v-if="isUserCenterOpen" class="modal">
      <div class="modal-content">
        <!-- <span class="close" @click="closeModal">&times;</span> -->
        <UserCenter />
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  import UserCenter from './UserCenter.vue';
  
  const isUserCenterOpen = ref(false);
  
  const closeModal = () => {
    isUserCenterOpen.value = false;
  };
  
  defineExpose({ isUserCenterOpen, closeModal });
  </script>
  
  <style scoped>
  .modal {
    display: flex;
    align-items: center;
    justify-content: center;
    position: fixed;
    z-index: 1000;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    overflow: auto;
    background-color: rgba(0, 0, 0, 0.5);
  }
  
  .modal-content {
    background-color: #fefefe;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
  }
  
  .close {
    position: absolute;
    top: 10px;
    right: 10px;
    font-size: 28px;
    cursor: pointer;
  }
  </style>
  