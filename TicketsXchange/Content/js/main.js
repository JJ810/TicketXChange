(function ($) {
 "use strict";
 	
	$( ".event-map" ).on('mouseleave', function( event ){
	  $('.event-map iframe').css("pointer-events", "none"); 
	});
	
	// Textbox Clear
	$( ".hasclear" ).on('keyup', function( event ){
	  var t = $(this);
	  t.next('span').toggle(Boolean(t.val()));
	});

	$(".clearer").hide($(this).prev('input').val());

	// Price Range Slider
	$("#price-range").slider({
		tooltip: 'always',
		tooltip_split: true,
		formatter: function(value) {
			return '$ ' + value;
		}
	});

	$(activate);
		function activate() {
			$('.event-tabs')
				.scrollingTabs({
				  scrollToTabEdge: true		  
				})
				.on('ready.scrtabs', function() {
				$('.tab-content').show();
			});
		}
	 
	$('.hero-2 .count-down').countdown('2017/04/26').on('update.countdown', function(event) {
	  var $this = $(this).html(event.strftime('<li>%D <span>day%!d</span></li>'
		+ '<li>%H <span>hours</span></li>'
		+ '<li>%M <span>minutes</span></li>'
		+ '<li>%S <span>seconds</span></li>'));
	});
	
	// The slider being synced must be initialized first
	$('#carousel').flexslider({
		animation: "slide",
		controlNav: false,
		animationLoop: false,
		slideshow: false,
		itemWidth: 160,
		itemMargin: 5,
		asNavFor: '#slider'
	});
	 
	$('#slider').flexslider({
		animation: "slide",
		controlNav: false,
		directionNav: false,
		animationLoop: false,
		slideshow: false,
		sync: "#carousel"
	}); 
	   
	$("a,section,div,span,li,input[type='text'],input[type='button'],tr,button").on("click", function(){
		
		if ($(this).hasClass("event-map")) { 
			$('.event-map iframe').css("pointer-events", "auto");
		}
		
		if ($(this).hasClass("select-seat")) { 
			$(this).siblings().removeClass("selected");
			$(this).addClass('selected');
		}
		
		if ($(this).hasClass("clearer")) { 
			$(this).prev('input').val('').focus();
			$(this).hide();
		}
		
		if ($(this).hasClass("qty-btn")) { 
			var $button = $(this);
			var oldValue = $button.closest('.qty-select').find("input.quantity-input").val();

			if ($button.text() === "+") {
				var newVal = parseFloat(oldValue) + 1;
			} else {
				// Don't allow decrementing below zero
				if (oldValue > 1) {
					var newVal = parseFloat(oldValue) - 1;
				} else {
					newVal = 1;
				}
			}
			$button.closest('.qty-select').find("input.quantity-input").val(newVal);
			return false;
		}
		
		if ($(this).hasClass("closecanvas")) { 
			$("body").removeClass("offcanvas-stop-scrolling");
		}
	});
	
	   

})(jQuery);