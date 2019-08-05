
$(function() {
    
    var $formLogin = $('#login-form');
    var $formLost = $('#lost-form');
    var $formRegister = $('#register-form');
    var $divForms = $('#div-forms');
    var $modalAnimateTime = 300;
    var $msgAnimateTime = 150;
    var $msgShowTime = 2000;

  
 
    $("form").submit(function () {
        switch(this.id) {
            case "login-form":
                var login_username=$('#login_username').val();
                var login_password=$('#login_password').val();
                $.ajax({
                    method: 'post',
                    url: 'api/login',
                    data: {
                        Email: login_username,
                        Password: login_password
                    },
                    success: function (response) {          
                        console.log(response);
                        $('#register_firstname').val('');
                        $('#register_lastname').val('');
                    },
                    error: function (error) {
                        console.log(error)
                    }
                })

                return false;
                break;
            case "lost-form":
                var $ls_email=$('#lost_email').val();
                if ($ls_email == "ERROR") {
                    msgChange($('#div-lost-msg'), $('#icon-lost-msg'), $('#text-lost-msg'), "error", "glyphicon-remove", "Send error");
                } else {
                    msgChange($('#div-lost-msg'), $('#icon-lost-msg'), $('#text-lost-msg'), "success", "glyphicon-ok", "Send OK");
                }
                return false;
                break;
            case "register-form":
                var register_firstname = $('#register_firstname').val();
                var register_lastname = $('#register_lastname').val();
                var register_email = $('#register_email').val();
                var register_password = $('#register_password').val();
                var register_confirmpassword = $('#register_confirmpassword').val();
                if (register_password != register_confirmpassword) {
                    alert("Password must be same.")
                } else {
                    $.ajax({
                        method: 'post',
                        url: 'api/register',
                        data: {
                            FirstName: register_firstname,
                            LastName: register_lastname,
                            Email: register_email,
                            Password: register_password
                        },
                        success: function (response) {
                            modalAnimate($formRegister, $formLogin);
                            $('#register_firstname').val('');
                            $('#register_lastname').val('');
                            $('#register_email').val('');
                            $('#register_password').val('');
                            $('#register_confirmpassword').val('');
                        },
                        error: function (error) {
                            console.log(error)
                        }
                    })
                }
            
                return false;
                break;
            default:
                return false;
        }
        return false;
    });
    
    $('#login_register_btn').click( function () { modalAnimate($formLogin, $formRegister) });
    $('#register_login_btn').click( function () { modalAnimate($formRegister, $formLogin); });
    $('#login_lost_btn').click( function () { modalAnimate($formLogin, $formLost); });
    $('#lost_login_btn').click( function () { modalAnimate($formLost, $formLogin); });
    $('#lost_register_btn').click( function () { modalAnimate($formLost, $formRegister); });
    $('#register_lost_btn').click( function () { modalAnimate($formRegister, $formLost); });
    
    function modalAnimate ($oldForm, $newForm) {
        var $oldH = $oldForm.height();
        var $newH = $newForm.height()+20;
        
        $oldForm.fadeToggle($modalAnimateTime, function(){
            $divForms.animate({height: $newH}, $modalAnimateTime, function(){
                $newForm.fadeToggle($modalAnimateTime);
            });
        });
    }
    
    function msgFade ($msgId, $msgText) {
        $msgId.fadeOut($msgAnimateTime, function() {
            $(this).text($msgText).fadeIn($msgAnimateTime);
        });
    }
    
    function msgChange($divTag, $iconTag, $textTag, $divClass, $iconClass, $msgText) {
        var $msgOld = $divTag.text();
        msgFade($textTag, $msgText);
        $divTag.addClass($divClass);
        $iconTag.removeClass("glyphicon-chevron-right");
        $iconTag.addClass($iconClass + " " + $divClass);
        setTimeout(function() {
            msgFade($textTag, $msgOld);
            $divTag.removeClass($divClass);
            $iconTag.addClass("glyphicon-chevron-right");
            $iconTag.removeClass($iconClass + " " + $divClass);
  		}, $msgShowTime);
    }
    $('#login-modal').on('hidden.bs.modal', function () {
        $('#register_firstname').val('');
        $('#register_lastname').val('');
        $('#register_email').val('');
        $('#register_password').val('');
        $('#register_confirmpassword').val('');
        $('#lost_email').val('');
        $('#login_username').val('');
        $('#login_password').val('');
        // do something…
    });
});
