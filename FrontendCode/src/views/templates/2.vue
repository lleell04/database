<!-- 图文显示的容器 -->
<!-- 特别的可手动调节的地方： -->
<!-- 1.背景颜色：背景图片有仙鹤祥云，并且图片也有颜色，大家可通过“特别修改1”处的覆盖层颜色进行调色 -->
<!-- 2.文字显示区域的滚动条：颜色、粗细均可按需调节 -->
<!-- 3.animate.css动画效果修改:参见https://animate.style/  -->
<!-- 4.其他,比如大小字体等均基础 -->
<template>
<div class="container-block">
	<img src="@/assets/mmy/blue_background.jpg">
	<div class="inner-block">
		<div class="slider-top-right">
			<div class="picture-and-text">
				<transition class="animate__animated animate__fadeInDown">
					<img class="picture" src="@/assets/mmy/product.png">
				</transition>
				<transition class="animate__animated animate__fadeInUp">
					<div class="text">
						<p>{{ text }}</p>
					</div>
				</transition>
			</div>
		</div>
	</div>
</div> 

</template>
  
<script setup>
import 'animate.css';
import { ref } from 'vue';

const text=ref('瓷器，也做“磁器” 。是由瓷石、高岭土、石英石、莫来石等烧制而成，外表施有玻璃质釉或彩绘的物器。瓷器的成形要通过在窑内经过高温（约1280℃～1400℃）烧制，瓷器表面的釉色会因为温度的不同从而发生各种化学变化，是中华文明展示的瑰宝。中国是瓷器的故乡，瓷器是古代劳动人民的一个重要的创造。谢肇淛在《五杂俎》记载：“今俗语窑器谓之磁器者，盖磁州窑最多，故相延名之，如银称米提，墨称腴糜之类也。”当时出现的以“磁器”代窑器是由磁州窑产量最多所致。这是迄今发现最早使用瓷器称谓的史料。');
</script>
<style scoped>
/* body {
	text-align: center;
} */
.animate__animated.animate__fadeInUp{
	--animate-duration: 1.5s;
}
.animate__animated.animate__fadeInDown{
	--animate-duration: 1.5s;
}
.container-block {
    overflow: hidden;
    border-radius: 15px;
	color: #fff;
	display: inline-block;
	margin: 2rem auto;
	max-width: 60%;
	position: relative;	
	&::before {
		/* 特别修改1 */
        /* 企图用覆盖层的颜色改变原本背景色,并保留原本图案*/
		/* background-color: rgba(130, 17, 31,0.3); */
        /* background-color: rgba(255, 255, 255,0.3); */
		bottom: 0;
		content: '';
		display: block;
		position: absolute;
		top: 0;
		width: 100%;
	}
	&:hover {
		.inner-block:before,
		.slider-top-right:after {
			height: 100%;
		}
		.inner-block:after,
		.slider-top-right:before {
			width: 100%;
		}
	}
	img {
		display: block;
		max-width: 100%;
	}
}

.slider-top-right:before,
.inner-block:after {
	height: 2px;
	transition: width .75s ease;
	width: 0%;
}

.slider-top-right:after,
.inner-block:before {
	height: 0%;
	transition: height .75s ease;
	width: 2px;
}

.inner-block:before,
.inner-block:after,
.slider-top-right:before,
.slider-top-right:after {
	background-color: #fff;
	content: '';
	display: block;
	position: absolute;
}

.inner-block {
	font-size: 2em;
	width: 90%;
	height: 90%;
	position: absolute;
	top: 0;
	left: 0;
	right: 0;
	bottom: 0;
	margin: auto;
	&:before {
		bottom: 0;
		left: 0;
	}
	&:after {
		bottom: 0;
		right: 0;
	}
}

.slider-top-right {
	position: relative;
	width: 100%;
	height: 100%;
	display: flex; /* 使用 Flexbox */
	justify-content: center; /* 水平居中 */
	align-items: center; /* 垂直居中 */
	&:before {
		top: 0;
		left: 0;
	}
	&:after {
		top: 0;
		right: 0;
	}
}
.picture-and-text{
	width: 90%;
	height:90%;
	display: flex; 
	flex-direction: row;
}
.picture{
	width:50%;
	border-radius: 20px;
}
.text{
	/* 华文宋 */
	width: 50%;
	font-family: 'Noto Serif SC', serif;
	font-size: 20px;
	padding: 10px 10px 20px 20px;
	overflow-y: auto; 
	max-height: 100%; /* 限制最大高度为父容器高度 */
	text-align: left;
	
}
/* 特别修改2 */
/* 自定义滚动条样式 */
.text::-webkit-scrollbar {
  width: 5px;
}

.text::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.text::-webkit-scrollbar-thumb {
  background: #5f697a;
  border-radius: 10px;
}

</style>