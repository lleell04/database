<template>
    

			<div class="page-container">
			<div class="loading-container">
    <div class="loader">
     
    </div>

    <p class="loading-text">  加载中，请稍候...</p>
  
</div>
</div>

  <div class="button-container">
        <button class="styled-button"><span>上一页</span></button>
        <button class="styled-button"><span>下一页</span></button>
    </div>


</template>
<style scoped>
.page-container {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    background-color: #ffffff; 
    font-family: Arial, sans-serif;
}

.loading-container {
            text-align: center;
            margin-bottom: 20px;
        }

        .loader {
            border: 12px solid #f5f5dc; 
            border-top: 12px solid #8B0000;
            border-radius: 50%;
            width: 80px;
            height: 80px;
            animation: spin 1.5s linear infinite;
            margin-bottom: 10px;
			justify-content: center;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }
            100% {
                transform: rotate(360deg);
            }
        }

       
        .loading-text {
            font-size: 16px;
            color: #555;
            margin: 0;
			text-align: center;
        }

/*按钮*/
		.button-container {
            text-align: center;
            margin-top: 20px;
        }
        .styled-button {
            background-color: #b33c43; /* 殷红 */
            color: #f9f9f9; /* 米白 */
            border: 2px solid #a02c36;
            border-radius: 8px;
            padding: 10px 20px;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
            transition: background-color 0.3s, transform 0.3s, box-shadow 0.3s;
            position: relative;
            overflow: hidden;
            margin: 0 10px;
        }
        .styled-button::before {
            content: '';
            position: absolute;
            top: 50%;
            left: 50%;
            width: 100%;
            height: 100%;
            border-radius: 8px;
            border: 2px solid #a02c36;
            background: transparent;
            box-shadow: 0 0 8px rgba(0, 0, 0, 0.2);
            transform: translate(-50%, -50%);
            pointer-events: none;
            z-index: 0;
        }
        .styled-button span {
            position: relative;
            z-index: 1;
        }
        .styled-button:hover {
            background-color: #a02c36;
            transform: scale(1.05);
        }
        .styled-button:active {
            background-color: #8b242d;
            transform: scale(0.95);
        }
</style>