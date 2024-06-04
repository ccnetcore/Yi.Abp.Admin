<template>
    <div class="start-body">

        <div class="content">
            <div class="content-title"><span>开始</span></div>
            <div class="content-body">
                <div class="content-body-left">

                    <h4>安装 Yi.Abp.Tool</h4>
                <p>如果之前未安装 Yi.Abp.Tool，请在命令行终端中安装：</p>
                </div>
            <div class="content-body-right">

            </div>


            </div>
        </div>

    </div>
</template>
<style lang="scss" scoped>
.start-body {
    height: 100%;
    width: 100%;
    background-color: #FCFCFC;

}

.content {
    width: 80%;
    margin: 0 auto;

    &-title {
        background-color: #FCFCFC;
        height: 100px;
        display: flex;
        align-items: center;

        span {
            color: #292d33;
            font-size: 48px;
            font-weight: 700;
        }
    }

    &-body {
        height: 2000px;
        padding: 48px;
        background-color: #fff;
        border-radius: 12px;
        border: 0;
        box-shadow: 0 0 1rem rgba(0, 0, 0, .08);
        display: flex;
        &-left{
            width: 75%;
        }
        &-right{
            width: 25%;
            background-color: #b84297;
            height: 1000px;
        }
        h4 {
            font-size: 20px;
            font-weight: 500;
            color: #b84297;
            margin-bottom: .25rem;
            margin-top: 0;
        
        }

        p {
            margin-top: 0;
            margin-bottom: 1rem;
        }
    }
}
</style>