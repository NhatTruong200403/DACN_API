﻿@model List<DoAnCNTT.Models.Post>
@using Microsoft.AspNetCore.Identity

@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager
@{
    ViewData["Title"] = "Index";
    var currentUser = await UserManager.GetUserAsync(User);
}

@{
    var notifications = Model.Where(p => p.IsDisabled == true && p.IsHidden == false && p.UserId == currentUser.Id).ToList();
}

<div class="tb">
    <h3>Thông báo</h3>
    <hr style="width:100%;" />
    <div class="tball">
        @if (notifications.Count == 0)
        {
            <div class="mesCha">
                <img />
                <div class="mesCon">
                    Bạn chưa có thông báo nào.
                </div>
            </div>
        }
        @foreach (var item in notifications)
        {

            <div class="mesCha">
                <img />
                <div class="mesCon">
                    Bài viết @item.Name đã bị báo cáo. Bạn đã vi phạm quy định @currentUser!.ReportPoint lần
                </div>
            </div>
        }

    </div>


</div>


@section Scripts {
    @{
        await Html.RenderPartialAsync("_ValidationScriptsPartial");
    }
}


<style>
    .tb {
        padding: 5px 0px;
        width: 350px;
        background-color: white;
        border-radius: 15px;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
    }

        .tb h3 {
            margin-top: 10px;
            padding-inline: 15px;
        }

    .tball {
        max-height: 400px;
        overflow-y: scroll;
    }

    .mesCha {
        margin: 2px;
        padding: 5px 10px;
        display: flex;
        background-color: white;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
    }

    .mesCon {
        padding: 2px 5px;
        font-size: 14px;
    }
</style>