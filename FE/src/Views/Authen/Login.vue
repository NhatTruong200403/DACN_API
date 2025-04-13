<template>
    <div class="ODN" v-bind:class="{ top: biendktc === 1 }">
        <!-- <div class="cangiua" v-if="bienkt === 2">
            <div class="thongbaothanhcong">Xin chào {{ tennd }}!</div>
        </div> -->
        <div class="cangiua" v-if="biendktc === 1">
            <div class="thongbaothanhcong">Đăng ký thành công!!! Hãy đăng nhập</div>
        </div>
        <div class="wrapper" v-if="bienkt === 1" v-bind:class="{ active: dai === true }">
            <span class="icon-close">
                <a class="custom-link" @click="Icon_close()">
                    <i class="ri-close-line"></i>
                </a>
                <!-- <a class="custom-link" href="javascript:history.go(-1);">
                    <i class="ri-close-line"></i>
                </a> -->
            </span>
            <div v-if="bienhien === true" class="form-box login">
                <h2>Đăng nhập</h2>
                <form @submit.prevent="postTKDN"> <!-- Ngăn chặn gửi form mặc định -->
                    <div class="input">
                        <span class="icon"><ion-icon name="mail-outline"></ion-icon></span>
                        <input type="email" v-model="EmailUser" autocomplete="username" required>
                        <label>Email</label>
                    </div>
                    <div class="input">
                        <span class="icon"><ion-icon name="lock-closed-outline"></ion-icon></span>
                        <input type="password" v-model="PassUser" autocomplete="current-password" required>
                        <label>Mật khẩu</label>
                    </div>
                    <div class="remember">
                        <label><input type="checkbox">Nhớ mật khẩu</label>
                        <a href="#" v-on:click="quenmk()">Quên mật khẩu?</a>
                    </div>
                    <div class="thongbao" v-if="biensai === 2">Email không chính xác!!!</div>
                    <div class="thongbao" v-if="biensai === 3">Password không chính xác!!!</div>
                    <button type="submit" class="btn">Đăng nhập</button> <!-- Không cần v-on:click ở đây nữa -->
                    <!-- <div class="login-register">
                        <p>Bạn không có tài khoản? <a v-on:click="hamnhan()" href="#" class="register-link">Đăng ký</a>
                        </p>
                    </div> -->
                    <div class="login-register" style="margin-top: 7px;">
                        <p>Bạn không có tài khoản? <a v-on:click="hamnhan()" href="#" class="register-link">Đăng ký</a>
                        </p>
                        <!-- <p class="hoac">Hoặc</p> -->
                    </div>
                    <!-- <div class="lienket">
                        <div style="padding: 6.5px 20px;border: 0.3px solid #919191c3; border-radius: 5px;">
                            <img width="45" height="45" style="margin-left: -20px;" src="https://img.icons8.com/color/48/google-logo.png" alt="google-logo"/>
                            <i>Tiếp tục với Google</i>
                        </div>
                        
                    </div> -->
                </form>
            </div>

            <div v-if="bienhien === false" class="form-box register">
                <h2>Đăng ký</h2>
                <form @submit.prevent="postTKDK"> <!-- Ngăn chặn hành vi gửi form -->
                    <div class="input">
                        <span class="icon"><ion-icon name="person-outline"></ion-icon></span>
                        <input type="text" required v-model="NameUser">
                        <label>Tên</label>
                    </div>
                    <div class="input">
                        <span class="icon"><ion-icon name="call-outline"></ion-icon></span>
                        <input type="tel" v-model="PhoneDK" pattern="[0-9]{10,11}" maxlength="11" required>
                        <label>Số điện thoại</label>
                    </div>
                    <div class="input">
                        <span class="icon"><ion-icon name="mail-outline"></ion-icon></span>
                        <input type="email" v-model="EmailDK" required>
                        <label>Email</label>
                    </div>
                    <div class="input">
                        <span class="icon"><ion-icon name="lock-closed-outline"></ion-icon></span>
                        <input type="password" v-model="PassDK" autocomplete="current-password" required>
                        <label>Mật khẩu</label>
                    </div>
                    <div class="remember">
                        <label><input type="checkbox" @change="checked = !checked" :checked="checked == true">Tôi đồng ý với các Điều khoản & Điều
                            kiện</label>
                    </div>
                    <div class="thongbao" v-if="biendung === true">Email đã được đăng ký!!!</div>
                    <button type="submit" class="btn" :disabled="checked === false"
                        :class="{ matchuot: biennutnho === false }">Đăng kí</button>
                    <div class="login-register">
                        <p>Bạn đã sẵn sàng để đăng nhập? <a v-on:click="hamnhan2()" href="#" class="login-link">Đăng
                                nhập</a></p>
                    </div>
                </form>
            </div>

            <div v-if="bienhien === 1" class="form-box not">
                <h2>Quên mật khẩu</h2>
                <form action="#">
                    <div class="input">
                        <span class="icon"><ion-icon name="person-outline"></ion-icon></span>
                        <input type="text" required v-model="User">
                        <label>Tên</label>
                    </div>
                    <div class="input">
                        <span class="icon"><ion-icon name="mail-outline"></ion-icon></span>
                        <input type="email" v-model="DKT" required>
                        <label>Email</label>
                    </div>
                    <div class="" v-if="from === true">
                        <div class="input">
                            <span class="icon"><ion-icon name="mail-outline"></ion-icon></span>
                            <input type="password" v-model="DKP" required>
                            <label>Nhập mật khẩu mới</label>
                        </div>
                        <div class="input">
                            <span class="icon"><ion-icon name="mail-outline"></ion-icon></span>
                            <input type="password" v-model="DKp" required>
                            <label>Xác nhận lại mật khẩu</label>
                        </div>
                    </div>
                    <div class="thongbao" v-if="biendung === true">Không hợp lệ!!!</div>
                    <div class="thongbao" v-if="biensaimk === true">Mật khẩu và xác nhận mật khẩu không trùng!!!</div>
                    <button type="submit" class="btn" v-on:click="postKttk()">Xác nhận</button>
                    <div class="login-register">
                        <p>Bạn đã sẵn sàng để đăng nhập? <a v-on:click="hamnhan2()" href="#" class="login-link">Đăng
                                nhập</a></p>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
import axios from 'axios';
import AuthenticationService from '../../Service/api/AuthenticationService';
export default {
    name: 'PlayersView',
    data() {
        return {
            biendangnhap: false,
            biensaimk: false,
            biendktc: 0,
            tennd: '',
            biensai: 3,
            biendung: false,
            bienkt: 1,
            bienhien: true,
            NameUser: '',
            PassDK: '',
            PhoneDK: '',
            EmailDK: '',
            EmailUser: '',
            PassUser: '',
            money: 0,
            avt: "",
            from: false,
            dai: false,
            user: null,
            dangnhapr: 1,
            checked: false
        }
    },
    props: {

    },
    methods: {
        async postTKDN() {
            try {
                // Gửi yêu cầu đăng nhập
                const response = await AuthenticationService.Login(this.EmailUser, this.PassUser);
                const token = response.data.message;
                console.log(response);
                var abc = this;
                if (token === "User not found") {
                    this.biensai = 2;
                    setTimeout(() => {
                        abc.biensai = 1;
                    }, 2000);
                } else if (token === "Wrong password") {
                    this.biensai = 3;
                    setTimeout(() => {
                        abc.biensai = 1;
                    }, 2000);
                } else if (response.data.success) {
                    sessionStorage.setItem('authToken', token);
                    console.log("Đã đăng nhập được user");
                    this.$emit('HamGetUser');
                    
                    
                }

                this.dangnhapr = 2; // Đánh dấu trạng thái đăng nhập thành công

            } catch (error) {
                console.error('Login failed:', error);
            }
        },

        async postTKDK() {
            try {
                const response = await AuthenticationService.SignUp(this.NameUser,this.EmailDK,this.PassDK, this.PhoneDK);
                const result = response.data.succeeded;
                console.log('Response:', result);
                if (result == true) {
                    this.biendktc = 1;
                    //tro ve trang dang nhap
                    this.bienhien = true;
                    this.dai = false
                }
                else {
                    console.log(response.data);
                }
            } catch (error) {
                console.error('SignUp failed:', error);
            }
        },
        Icon_close() {
            this.$emit('HamXoaLogin');
        },

        hamnhan() {
            this.bienhien = false;
            this.dai = true;
        },
        hamnhan2() {
            this.bienhien = true;
            this.dai = false
        },


        quenmk() {
            if (this.bienhien === true) {
                this.bienhien = 1;
                this.dai = true;
            }
        }

    },
   
}
</script>

<style>
.custom-link {
    color: rgb(255, 255, 255);
    /* Thay đổi màu sắc theo ý muốn */
    text-decoration: none;
    /* Bỏ dấu gạch dưới */
}

.hoac {
    margin-bottom: -5px;
    margin-top: 0px;
}

.lienket {
    text-align: center;
    margin-top: -5px;
    margin-bottom: -15px;
}

.lienket img {
    width: 40px;
    height: 40px;
    border: none;
    margin-inline: 20px;
    border-radius: 50px;
}

.top {
    margin-top: 250px;
}

.cangiua {
    background-color: #ffffff20;
    text-align: center;
    padding: 12px;
    margin-top: -257px;
    border-radius: 10px;
}

.thongbaothanhcong {
    color: rgb(255, 255, 255);
}

.thongbao {
    color: rgb(142, 16, 16);
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    font-size: 15px;
    text-decoration: underline;
    margin-bottom: 5px;
    margin-top: -5px;
}

.btn.matchuot {
    cursor: not-allowed;
}

.wrapper {
    position: relative;
    width: 400px;
    height: 410px;
    /* height: 480px; */
    background: rgba(255, 255, 255, 0.664);
    border: 2px solid rgba(255, 255, 255, 0.5);
    border-radius: 20px;
    backdrop-filter: blur(20px);
    box-shadow: 0 0 30px rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    overflow: hidden;
    transition: transform .5s ease, height .2s ease;
}

.wrapper.active {
    margin-top: 20px;
    height: 550px;
}
span{
    margin-inline: 0 !important;
}
.wrapper .form-box.login {
    transition: transform .18s ease;
    transform: translateX(0);
}

.wrapper.active .form-box.login {
    transition: none;
    transform: translateX(-400px);
}

.wrapper .form-box.register {
    position: absolute;
    transition: none;
    transform: translateX(400px);
}

.wrapper.active .form-box.register {
    transition: none;
    transform: translateX(0);
}

.wrapper .icon-close {
    position: absolute;
    top: 0;
    right: 0;
    width: 45px;
    height: 45px;
    background: #162938;
    font-size: 2em;
    color: #fff;
    display: flex;
    justify-content: center;
    align-items: center;
    border-bottom-left-radius: 20px;
    cursor: pointer;
    z-index: 1;
}

.form-box {
    width: 100%;
    padding: 40px;
}

.form-box h2 {
    font-size: 2em;
    color: #162938;
    text-align: center;
}

.input {
    position: relative;
    width: 100%;
    height: 50px;
    border-bottom: 2px solid #162938;
    margin: 30px 0;
}

.input label {
    position: absolute;
    top: 50%;
    left: 5px;
    transform: translateY(-50%);
    font-size: 1em;
    color: #162938;
    font-weight: 500;
    pointer-events: none;
    transition: 0.5s;
}

.input input:focus~label,
.input input:valid~label {
    top: -5px;
}

.input input {
    width: 100%;
    height: 100%;
    background: transparent;
    border: none;
    outline: none;
    font-size: 1em;
    color: #162938;
    font-weight: 600;
    padding: 0 35px 0 5px;
}

.input .icon {
    position: absolute;
    right: 8px;
    font-size: 1.2em;
    color: #162938;
    line-height: 57px;
}

.remember {
    font-size: 0.9em;
    color: #162938;
    font-weight: 500;
    margin: -15px 0 15px;
    display: flex;
    justify-content: space-between;
}

.remember label input {
    color: #162938;
    margin-right: 4px;
}

.remember a {
    color: #162938;
    text-decoration: none;
}

.remember a:hover {
    text-decoration: underline;
}

.btn {
    width: 100%;
    height: 45px;
    background: #162938;
    border: none;
    outline: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 1em;
    color: #fff;
    font-weight: 500;
}

.login-register {
    font-size: 0.9em;
    color: #162938;
    text-align: center;
    font-weight: 500;
    margin: 25px 0 10px;
}

.login-register p a {
    color: #162938;
    text-decoration: none;
    font-weight: 600;
}

.login-register p a:hover {
    text-decoration: underline;
}
</style>