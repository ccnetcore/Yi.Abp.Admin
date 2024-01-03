namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Argee
{
    public class AgreeDto
    {
        public AgreeDto(bool isAgree)
        {
            IsAgree = isAgree;
            if (isAgree)
            {

                Message = "点赞成功，点赞+1";
            }
            else
            {

                Message = "取消点赞，点赞-1";
            }

        }

        public bool IsAgree { get; set; }
        public string Message { get; set; }
    }
}
