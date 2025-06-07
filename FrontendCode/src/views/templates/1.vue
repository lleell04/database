<template>
    <div class="container">
			
			
			<section class="Gio-98">
				<div class="grid">
					<div class="box">
						<svg xmlns="http://www.w3.org/2000/svg" width="100%" height="100%">
							<line class="top" x1="0" y1="0" x2="900" y2="0"/>
							<line class="left" x1="0" y1="460" x2="0" y2="-920"/>
							<line class="bottom" x1="300" y1="460" x2="-600" y2="460"/>
							<line class="right" x1="300" y1="0" x2="300" y2="1380"/>
						</svg>
						<h3>文本框悬浮效果</h3>
						<span>文本2</span>
						<span></span>
				</div>
				</div>

			</section>
			
			</div>



</template>

<style scoped>

.container{
	background-color: #b9af8a;
}
.grid{
	background-color: #394a5e;
	width:300px;
}
.box {
	color:#FFF;
	width: 300px;
	height: 460px;
	position: relative;
	background: rgba(0,0,0,0.2);
	display: inline-block;
	/* cursor: pointer; */
	-webkit-transition: background 0.3s;
	transition: background 0.3s;

}
.box:hover {
	background: rgba(0,0,0,0.4);
	-webkit-transition-delay: 0s;
	transition-delay: 0s;
}
.box h3 {
	/* font-family: "Ruthie", cursive; */
	font-size: 30px;
	line-height: 370px;
	margin: 0;
	font-weight: 400;
	width: 100%;
}
.box span {
	display: block;
	font-weight: 400;
	text-transform: uppercase;
	letter-spacing: 1px;
	font-size: 13px;
	padding: 5px;
}
.box h3,
.box span {
	-webkit-transition: color 0.4s 0.5s;
	transition: color 0.4s 0.5s;
}
.box svg {
	position: absolute;
	top: 0;
	left: 0;
}
.box svg line {
	stroke-width: 3;
	stroke: #ecf0f1;
	fill: none;
	-webkit-transition: all .5s ease-in-out;
	transition: all .5s ease-in-out;
}
.box:hover svg line {
	stroke-width: 10;
	-webkit-transition-delay: 0s;
	transition-delay: 0s;
}
.box svg line.top,
.box svg line.bottom {
	stroke-dasharray: 330 240; 
}

.box svg line.left,
.box svg line.right {
	stroke-dasharray: 490 400;
}

.box:hover svg line.top {
	-webkit-transform: translateX(-300px);
	transform: translateX(-300px);
}

.box:hover svg line.bottom {
	-webkit-transform: translateX(300px);
	transform: translateX(300px);
}

.box:hover svg line.left {
	-webkit-transform: translateY(460px);
	transform: translateY(460px);
}

.box:hover svg line.right {
	-webkit-transform: translateY(-460px);
	transform: translateY(-460px);
} 
section {
	padding: 4em 2em;
	text-align: center;
}



</style>