<template>

    <div class="containerFa">
        <div class="child" v-if="checkBooking != true">
            <h1>Danh sách bài đăng của bạn</h1> <button class="btn nutshort" style="display: flex;" @click="checkBooking = !checkBooking;">
                 <label>Yêu cầu thuê xe</label> <i v-if="isPending > 0" style="display: block;width: 20px;height: 20px;font-size: 12px;border-radius: 50%;background-color: red;align-content: center;margin-top: 3px;margin-left: 10px;color: azure;">{{ isPending }}</i> </button>
        </div>
        <div class="child" v-else>
            <h1>Danh sách yêu cầu thuê xe </h1><button class="btn nutshort" @click="checkBooking = !checkBooking;">Danh
                sách xe</button>
        </div>

        <button v-if="checkBooking != true" class="btn nutshort" style="margin-bottom: 20px;"
            @click="this.$router.push({ name: 'user-post-create' });">Tạo bài đăng</button>


        <BookingPening v-if="checkBooking == true" />

        <div class="containerPost1" v-else>
            <router-link :to="{ name: 'user-post-detail', params: { id: item.id } }" class="box postcar"
                v-for="item in posts" :key="item.id">
                <div class="pta" style="width: 280px;height:180px; overflow: hidden;">
                    <img :src="'http://localhost:5027/' + item.image" :alt="item.image"
                        style="object-fit: cover; width: 100%; height: 100%;box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);" />
                </div>
                <div class="ptb">
                    <div v-if="item.gear" class="ptb1 apt" style="padding:2px 4px;">Số tự động</div>
                    <div v-else class="ptb1 apt" style="padding:2px 4px;background-color:darkkhaki;">Số sàn
                    </div>
                </div>

                <div class="ptc">
                    <a style="cursor: pointer;" asp-area="Customer" asp-controller="Posts" asp-action="Details"
                        asp-route-id="@item.Id">{{ item.name }}</a>
                </div>
                <div class="ptd">
                    <img src="/src/assets/logoWeb/road-map-line.svg" class="icon_map" alt="">
                    <span class="text_map">{{ item.rentLocation }}</span>
                </div>
                <hr>
                <div class="pte">
                    <div class="pte_left">
                        <!-- @* Chưa xong *@ -->
                        <div class="dgsao">
                            <img src="/src/assets/logoWeb/star-s-fill.svg" class="icon_sao_danhgia" alt="">
                            <span class="text_saodanhgia">{{ formatDecimal(item.avgRating, 1) }}</span>
                        </div>

                        <div v-if="item.rideNumber > 0" class="sochuyen">
                            <img src="/src/assets/logoWeb/luggage-cart-line.svg" class="icon_map" alt="">
                            <div class="text_sochuyen">{{ item.rideNumber }} chuyến</div>
                        </div>

                        <div v-else class="sochuyen">
                            <div class="text_sochuyen">Chưa có chuyến</div>
                        </div>


                    </div>
                    <div class="pte_right">
                        <div class="giagiam">{{ formatPrice(item.pricePerHour) }}/giờ</div>
                        <div class="giagiam">{{ formatPrice(item.pricePerDay) }}/ngày</div>
                    </div>
                </div>
            </router-link>

        </div>


    </div>

</template>

<script>
import PostService from '../../../Service/api/PostService';
import BookingPening from './Booking/BookingPening.vue';
export default {
    components: {
        BookingPening
    },
    data() {
        return {
            posts: [],
            checkBooking: false,
            isPending: 0,
            bookingPending:[]
        }
    },
    methods: {
        async getPost() {
            try {
                const response = await PostService.getAllPostByUserId();
                if (response != null) {
                    console.log(response);
                    this.posts = response.data;
                    console.log(this.posts);
                }
            }
            catch (error) {
                console.error('Lỗi lấy dữ liệu:', error);
            }
        },
        formatPrice(price) {
            if (price >= 1000000) {
                return (price / 1000000).toFixed(1).replace('.0', '') + "Tr";
            } else if (price >= 1000) {
                return (price / 1000).toLocaleString('en').replace(/,/g, '.') + "k";
            } else {
                return price.toString();
            }
        },
        formatDecimal(number, decimalPlaces) {
            const factor = Math.pow(10, decimalPlaces);
            return Math.ceil(number * factor) / factor;
        },
        async CheckBooking(){
            this.checkBooking = !this.checkBooking;
            try{
                const response = await BookingService.GetAllPenDing();    
                if(response.data.length > 0){
                    this.isPending = response.data.length;
                }           
                console.log(response.data);
            }
            catch(error){
                console.log("Lỗi lấy dữ liệu: ",error);
            }
            console.log("hehe",this.checkBooking);
        },
    },
    created() {
        this.getPost();
    }
}
</script>


<style>
.containerFa {
    padding: 20px 30px;
}

.containerFa .child {
    display: flex;
    justify-content: space-between;
}
</style>



<!-- 
@model IEnumerable<DoAnCNTT.Models.Post>

@{
    ViewData["Title"] = "Danh sách bài đăng";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

@*
<h1>Danh sách bài đăng</h1>


<table class="table">
    <thead>
        <tr>
            <th>
                Tên
            </th>
            <th>
                Hình ảnh
            </th>
            <th>
                Mô tả
            </th>
            <th>
                Địa chỉ nhận
            </th>
            <th>
                Có tài xế
            </th>
            <th>
                Giá
            </th>
            <th>
                Hộp số
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Name)
            </td>
            <td>
                <img src="@item.Image" alt="hinhanh.jpg" width="200px" />
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Description)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.RentLocation)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.HasDriver)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Price)
            </td>
            <td>
                @if (item.Gear)
                {
                    <text>Số tự động</text>
                }
                else
                {
                    <text>Số sàn</text>
                }
            </td>
            <td>

                <a asp-action="Edit" asp-route-id="@item.Id">Sửa</a> |
                <a asp-action="Details" asp-route-id="@item.Id">Chi tiết</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Xóa</a>
            </td>
        </tr>
}
    </tbody>
</table>

*@


<section class="py-2" >
    <div style="font-size:20px;background-color:#1ee156;padding:20px 30px;border-radius:15px;">
        <div style ="font-size:20px;color:white;display:flex;">
            <img class="logo" src="@Url.Content("~/images/logo/carrrr.png")" alt="" style="margin-right:20px;" />
            <div style="margin-top:15px;">
                <i>Bạn có xe cho thuê?</i>
                <div style="margin-left:-15px;">
                    <a asp-area="Customer" asp-controller="Posts" asp-action="Create">Cho thuê xe ngay tại đây. </a>
                </div>
            </div>
       
        </div>
        
        
    </div>
    <h1>Danh sách xe cho thuê của bạn</h1>
    <div class="container ">
        <div class="row gx-4 gx-lg-5 row-cols-2 row-cols-md-3 row-cols-xl-4 justify_content-center">
            
            @foreach (var item in Model)
            {

                    <div class="box" style="padding:3px;margin-left:20px;background-color:white;border:0.5px solid black;margin-bottom:15px;">
                        <div class="pta" style="width: 280px;height:180px; overflow: hidden;">
                            <img src="@item.Image" alt="" style="object-fit: cover; width: 100%; height: 100%;" />
                        </div>
                        <div class="ptb">
                            @if (item.Gear)
                            {
                                <div class="ptb1 apt" style="padding:2px 4px;">Số tự động</div>
                            }
                            else
                            {
                                <div class="ptb1 apt" style="padding:2px 4px;">Số sàn</div>
                            }

                        </div>

                        <div class="ptc"><a asp-area="Customer" asp-controller="Posts" asp-action="Details" asp-route-id="@item.Id">@Html.DisplayFor(modelItem => item.Name)</a></div>
                        <div class="ptd">
                            <img src="@Url.Content("~/images/logo/road-map-line.svg")" class="icon_map" alt="">
                            <span class="text_map">@Html.DisplayFor(modelItem => item.RentLocation)</span>
                        </div>
                        <hr>
                        <div class="pte">
                            <div class="pte_left">
                                @*  Chưa xong *@
                                <div class="dgsao">
                                    <img src="@Url.Content("~/images/logo/star-s-fill.svg")" class="icon_sao_danhgia" alt="">
                                    <span class="text_saodanhgia">@item.AvgRating</span>
                                </div>
                                @if (item.RideNumber > 0)
                                {
                                    <div class="sochuyen">
                                        <img src="@Url.Content("~/images/logo/luggage-cart-line.svg")" class="icon_map" alt="">
                                        <div class="text_sochuyen">@item.RideNumber chuyến</div>
                                    </div>
                                }
                                else
                                {
                                    <div class="sochuyen">
                                        Chưa có chuyến
                                    </div>
                                }

                            </div>
                            <div class="pte_right">
                                <div class="giagiam">@item.Price/giờ</div>
                            </div>
                        </div>

                    </div>
            }
        </div>
    </div>
</section>

<script>
    function formatPrice(price) {
        if (price >= 1000) {
            return (price / 1000) + 'k';
        }
        return price;
    }

    $(document).ready(function () {
        // Định dạng tất cả các phần tử có class 'giagiam'
        $('.giagiam').each(function () {
            var price = parseFloat($(this).text().replace(/[^0-9.-]+/g, ""));
            $(this).text(formatPrice(price) + '/giờ');
        });
    });
</script>

<style>
    .box {
        width: 305px;
        height: 345px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        border-radius: 15px;
    }

    .pta {
        padding: 0px;
        margin: 10px;
    }

        .pta img {
            border-radius: 15px;
            width: 280px;
            height: 180px;
        }

    .ptb {
        padding: 0px;
        display: flex;
        width: 100%;
        margin: 0;
    }

    .ptb1 {
        font-size: 13px;
        border-radius: 50px;
        margin: 0;
    }

        .ptb1.apt {
            background-color: aquamarine;
        }

        .ptb1.bpt {
            background-color: rgb(243, 253, 49);
        }

        .ptb1.cpt {
            background-color: rgb(79, 143, 255);
        }

    .ptb div {
        padding: 0px;
        margin-inline: 10px;
    }

    .ptc {
        padding: 0px;
        margin-block: 7px;
    }

    .ptd {
        padding: 0px;
        margin-inline: 10px;
        display: flex;
    }

        .ptd .icon_map {
            width: 25px;
        }

        .ptd .text_map {
            padding-top: 3px;
        }

    hr {
        width: 275px;
        margin-top: 10px;
        margin-bottom: 5px;
    @* margin:5px; *@
    }

    .pte {
        padding: 0px;
        margin-inline: 10px;
        display: flex;
        justify-content: space-between;
    }

        .pte .pte_left {
            display: flex;
        }

            .pte .pte_left .dgsao {
                display: flex;
                margin-right: 10px;
            }

            .pte .pte_left .sochuyen {
                display: flex;
            }

                .pte .pte_left .sochuyen .icon_map {
                    width: 19px;
                }

            .pte .pte_left .dgsao .icon_sao_danhgia {
                width: 18px;
                /* color: rgb(253, 236, 0); */
            }

        .pte .pte_right {
            display: flex;
        }

            .pte .pte_right div {
                margin-right: 10px;
            }

</style> -->