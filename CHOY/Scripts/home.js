var bar=document.getElementById('bar');

var btn_login = document.getElementById('login_btn');
var btn_register = document.getElementById("reg_btn");
var btn_close = document.getElementById("colse_btn");			

var bar_login=btn_login.parentNode.classList;
var bar_register=btn_register.parentNode.classList;
var bar_close=btn_close.parentNode.classList;

var page_reg=document.getElementById("page_reg");
var page_home=document.getElementById("page_home");
var page_login=document.getElementById("page_login");
var page_check=document.getElementById("page_check");



//左方按鈕切換監聽
bar.addEventListener('click',objEvent);

function objEvent(obj){
	switch(obj.target.id){
		
		case"reg_btn":
			register(obj.target);
			break;
		
		case"login_btn":
			login(obj.target);
			break;
		
		case"colse_btn":
			close(obj.target);
			break;
	}
}



function register(){
	page_reg.classList.add('show_anima');
// 	// page_reg.classList.remove('hidden');
// 	// page_reg.style.display="block";
	page_reg.classList.remove('hidden_anima');
}


function login(){
	page_login.classList.add('show_anima');
// 	// page_login.classList.remove('hidden');
	page_login.classList.remove('hidden_anima');
// 	// page_login.style.display='block';

}

function close(obj){

	if(bar_register=='btn1'){

		page_login.classList.add('hidden_anima');
		page_login.classList.remove('show_anima');
		page_login.style.display='none';

	}
	else{

		page_reg.classList.add('hidden_anima');
		page_reg.classList.remove('show_anima');
		page_reg.style.display='none';
		
	}
}


/////////////////頁面跳轉(利用 #)////////////////////////
function renderByUrl(url) {
    //若沒有hash值預設導向至#/
    if (location.hash === "") {
      location.href = "#/home";
      return;
    }
    //路徑對應頁面內容
    if (url === "#/" || url === "#/home") {
		page_home.style.display='block';
		page_login.style.display='none';
		page_reg.style.display='none';
		page_check.style.display='none';

		bar_close.remove('btn1','btn2','hidden');
		bar_login.remove('btn1','btn2','hidden');
		bar_register.remove('btn1','btn2','hidden');
		bar_login.add('btn1');
		bar_register.add('btn2');
		bar_close.add('hidden');

    } else if (url === "#/login") {
      	page_home.style.display='none';
		page_login.style.display='block';
		page_reg.style.display='none';
		page_check.style.display='none';

		bar_close.remove('btn1','btn2','hidden');
		bar_login.remove('btn1','btn2','hidden');
		bar_register.remove('btn1','btn2','hidden');
		bar_register.add('btn1');
		bar_close.add('btn2');
		bar_login.add('hidden');

    } else if (url === "#/register") {
      	page_home.style.display='none';
		page_login.style.display='none';
		page_reg.style.display='block';
		page_check.style.display='none';

		bar_close.remove('btn1','btn2','hidden');
		bar_login.remove('btn1','btn2','hidden');
		bar_register.remove('btn1','btn2','hidden');
		bar_login.add('btn1');
		bar_close.add('btn2');
		bar_register.add('hidden');
    }else if (url === "#/check"){
    	page_home.style.display='none';
		page_login.style.display='none';
		page_reg.style.display='none';
		page_check.style.display='block';
    }
  }

  //載入事件
  window.addEventListener("load", function () {
    //使用location.hash判斷頁面內容
    renderByUrl(location.hash);
  });

  //監聽網址改變事件
  window.addEventListener("hashchange", function () {
    //使用location.hash判斷頁面內容
    renderByUrl(location.hash);
  });


////////////生日日期限制/////////////////////////
var today = new Date();
var dd = today.getDate();
var mm = today.getMonth()+1; //January is 0!
var yyyy = today.getFullYear();
if(dd<10){
    dd='0'+dd;
} 
if(mm<10){
    mm='0'+mm;
} 

today = yyyy+'-'+mm+'-'+dd;
document.getElementById("BDay").setAttribute("max", today);