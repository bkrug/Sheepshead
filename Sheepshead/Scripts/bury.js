$(document).ready(function() 
{
    var buryPage = new BuryPage();
    $('#bury-btn').click(buryPage.OnBuryClick);
});

var BuryPage = function () {
    this.OnNoClick = function (e) {
        $('#id').val(1);
        $('#willPick').val(false);
        $('form').submit();
    };

    this.OnBuryClick = function (e) {
        var dropCards = [];
        $('.js-card-cb').each(function () {
            if ($(this).is(':checked'))
                dropCards.push($(this).data('index'));
        });
        if (dropCards.length == 2) {
            $('#id').val(1);
            $('#droppedCardIndicies').val(dropCards.join(';'));
            $('form').submit();
        }
        else {
            alert("Must drop exactly two cards.");
        }
    };
};