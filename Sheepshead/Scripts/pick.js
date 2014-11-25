$(document).ready(function() 
{
    var pickPage = new PickPage();
    $('#no-btn').click(pickPage.OnNoClick);
    $('#yes-btn').click(pickPage.OnYesClick);
});

var PickPage = function () {
    this.OnNoClick = function (e) {
        $('#willPick').val(false);
        $('form').submit();
    };

    this.OnYesClick = function (e) {
        $('#willPick').val(true);
        $('form').submit();
    };
};