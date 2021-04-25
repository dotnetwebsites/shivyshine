namespace Shivyshine.Utilities
{
    public class MailContent
    {
        public string Content { get; set; }

        public MailContent()
        {
            Content = "default test mail";
        }

        public MailContent(string logoName, string title, string url, string buttonValue)
        {
            Content = @"<div><u></u><div 
                style='width:100%!important;height:100%;font-family:'Rubik','Google Sans',Helvetica,Arial,sans-serif;background-color:#f8f9fa;color:#868e96;margin:0' 
                bgcolor='#f8f9fa'><table width='100%' cellpadding='0' cellspacing='0' role='presentation' 
                style='width:100%;background-color:#f8f9fa;margin:0;padding:0' bgcolor='#f8f9fa'><tbody><tr><td align='center' 
                style='word-break:break-word;font-family:'Rubik','Google Sans',Helvetica,Arial,sans-serif;font-size:16px'>
                <table width='100%' cellpadding='0' cellspacing='0' role='presentation' style='width:100%;margin:0;padding:0'>
                <tbody>
                <tr><td style='word-break:break-word;font-family:'Rubik','Google Sans',Helvetica,Arial,sans-serif;font-size:16px;text-align:center;color:#868e96!important;padding:25px 0' align='center'> 
                
                <a href='http://shivyshine.com/' style='color:#868e96!important;text-decoration:none;font-size:16px;font-weight:bold' target='_blank'> 
                <h2>" + logoName + @"</h2></a>
                
                </td></tr><tr><td width='570' cellpadding='0' cellspacing='0' 
                style='word-break:break-word;font-family:'Rubik','Google Sans',Helvetica,Arial,sans-serif;font-size:16px;width:100%;margin:0;padding:0'>
                <table align='center' width='570' cellpadding='0' 
                cellspacing='0' role='presentation' style='width:570px;border-radius:5px;background-color:#ffffff;margin:0 auto;padding:0' 
                bgcolor='#FFFFFF'><tbody><tr>
                <td style='word-break:break-word;font-family:'Rubik','Google Sans',Helvetica,Arial,sans-serif;font-size:16px;text-align:center;padding:60px 45px 25px' align='center'>
                <div><span>
                
                <h1>" + title + @"</h1>

                <div>            
                <hr style='border-collapse: collapse;border: 1px solid lightgrey;' />
                </div>
                <div style='font-family:'Rubik','Google Sans',Helvetica,Arial,sans-serif;font-size:20px;line-height:30px;color:#414141'> 
                <h3>
                Thanks for choosing <span style='font-weight:bold;color:#7bb0f4'>
                " + logoName + @"
                </span>
                </h3>
                
                </div><div><br></div><div>Simply click on the button below, and you will be automatically connected.
                </div><div><br></div> </span><div> 
                
                <a href='" + url + @"' style='text-decoration:none;
                display: inline-block;font-weight: 400;text-align: center;white-space: nowrap;vertical-align: middle;
                -webkit-user-select: none;-moz-user-select: none;-ms-user-select: none;user-select: none;
                border: 1px solid transparent;padding: .375rem .75rem;font-size: 1rem;line-height: 1.5;
                border-radius: .25rem;transition: color .15s ease-in-out, background-color .15s ease-in-out, border-color .15s ease-in-out, box-shadow .15s ease-in-out;
                color: #fff;background-color: #007bff;border-color: #007bff;padding: .5rem 1rem;font-size: 1.25rem;line-height: 1.5;border-radius: .3rem;'                 
                target='_blank'>" + buttonValue + @"</a>

                <br></div><span><div><br></div>
                <div>If you didn't request this email, simply ignore it, nothing has been changed on your account :)</div>
                <div><br></div><div>All the best from " + logoName + @"</div> </span></div></td></tr></tbody></table>
                </td></tr><tr><td style='word-break:break-word;font-family:'Rubik','Google Sans',Helvetica,Arial,sans-serif;font-size:16px'>
                <table align='center' width='570' cellpadding='0' cellspacing='0' role='presentation' style='width:570px;text-align:center;margin:0 auto;padding:0'>
                <tbody><tr><td align='center' 
                style='word-break:break-word;font-family:'Rubik','Google Sans',Helvetica,Arial,sans-serif;font-size:16px;padding:45px'>
                <p style='font-size:13px;line-height:1.625;text-align:center;color:#a9b1b9;margin:.4em 0 1.8em' align='center'> 
                " + logoName + @" <br>45, Dwarka Nagar, Coach Factory Road, Bhopal, India</p></td></tr></tbody></table></td></tr>
                </tbody></table></td></tr></tbody></table></div></div>";
        }

    }
}