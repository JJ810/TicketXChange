/* tooltip.js - tooltip functionality
   requires areacorners.js
*/

(function ($) {

    var m = $.mapster, u = m.utils;
    
    $.extend(m.defaults, {
        toolTipContainer: '<div class="select-popup"></div>',
        showToolTip: true,
        toolTipFade: true,
        toolTipClose: ['area-mouseout','image-mouseout'],
        onShowToolTip: null,
        onHideToolTip: null
    });
    
    $.extend(m.area_defaults, {
        toolTip: null,
        toolTipClose: null
    });
    

    /**
     * Show a tooltip positioned near this area.
     * 
     * @param {string|jquery} html A string of html or a jQuery object containing the tooltip content.
     * @param {string|jquery} [template] The html template in which to wrap the content
     * @param {string|object} [css] CSS to apply to the outermost element of the tooltip 
     * @return {jquery} The tooltip that was created
     */
    
    function createToolTip(html, template, css) {
        var tooltip;

        // wrap the template in a jQuery object, or clone the template if it's already one.
        // This assumes that anything other than a string is a jQuery object; if it's not jQuery will
        // probably throw an error.
        
        if (template) {
            tooltip = typeof template === 'string' ?
                $(template) : 
                $(template).clone();

            tooltip.append(html);
        } else {
            tooltip=$(html);
        }

        // always set display to block, or the positioning css won't work if the end user happened to
        // use a non-block type element.

        tooltip.css($.extend((css || {}),{
                display:"block",
                position:"absolute"
            })).hide();
        
        $('body').append(tooltip);

        // we must actually add the tooltip to the DOM and "show" it in order to figure out how much space it
        // consumes, and then reposition it with that knowledge.
        // We also cache the actual opacity setting to restore finally.
        
        tooltip.attr("data-opacity",tooltip.css("opacity"))
            .css("opacity",0);
        
        // doesn't really show it because opacity=0
        
        return tooltip.show();
    }


    /**
     * Show a tooltip positioned near this area.
     * 
     * @param {jquery} tooltip The tooltip
     * @param {object} [options] options for displaying the tooltip.
     *  @config {int} [left] The 0-based absolute x position for the tooltip
     *  @config {int} [top] The 0-based absolute y position for the tooltip
     *  @config {string|object} [css] CSS to apply to the outermost element of the tooltip 
     *  @config {bool} [fadeDuration] When non-zero, the duration in milliseconds of a fade-in effect for the tooltip.
     */
    
    function showToolTipImpl(tooltip,options)
    {
        var tooltipCss = { 
                "left":  options.left + "px",
                "top": options.top + "px"
            }, 
            actalOpacity=tooltip.attr("data-opacity") || 0,
            zindex = tooltip.css("z-index");
        
        if (parseInt(zindex,10)===0 
            || zindex === "auto") {
            tooltipCss["z-index"] = 9999;
        }

        tooltip.css(tooltipCss)
            .addClass('mapster_tooltip');

        
        if (options.fadeDuration && options.fadeDuration>0) {
            u.fader(tooltip[0], 0, actalOpacity, options.fadeDuration);
        } else {
            u.setOpacity(tooltip[0], actalOpacity);
        }
    }
      
    /**
     * Hide and remove active tooltips
     * 
     * @param  {MapData} this The mapdata object to which the tooltips belong
     */
    
    m.MapData.prototype.clearToolTip = function() {
        if (this.activeToolTip) {
            this.activeToolTip.stop().remove();
            this.activeToolTip = null;
            this.activeToolTipID = null;
            u.ifFunction(this.options.onHideToolTip, this);
        }
    };

    /**
     * Configure the binding between a named tooltip closing option, and a mouse event.
     *
     * If a callback is passed, it will be called when the activating event occurs, and the tooltip will
     * only closed if it returns true.
     *
     * @param  {MapData}  [this]     The MapData object to which this tooltip belongs.
     * @param  {String}   option     The name of the tooltip closing option
     * @param  {String}   event      UI event to bind to this option
     * @param  {Element}  target     The DOM element that is the target of the event
     * @param  {Function} [beforeClose] Callback when the tooltip is closed
     * @param  {Function} [onClose]  Callback when the tooltip is closed
     */
    function bindToolTipClose(options, bindOption, event, target, beforeClose, onClose) {
        var event_name = event + '.mapster-tooltip';
        
        if ($.inArray(bindOption, options) >= 0) {
            target.unbind(event_name)
                .bind(event_name, function (e) {
                    if (!beforeClose || beforeClose.call(this,e)) {
                        target.unbind('.mapster-tooltip');
                        if (onClose) {
                            onClose.call(this);
                        }
                    }
                });
            
            return {
                object: target, 
                event: event_name
            };
        }
    }
    
    /**
     * Show a tooltip.
     *
     * @param {string|jquery}   [tooltip]       A string of html or a jQuery object containing the tooltip content.
     * 
     * @param {string|jquery}   [target]        The target of the tooltip, to be used to determine positioning. If null,
     *                                          absolute position values must be passed with left and top.
     *
     * @param {string|jquery}   [image]         If target is an [area] the image that owns it
     * 
     * @param {string|jquery}   [container]     An element within which the tooltip must be bounded
     *
     *
     * 
     * @param {object|string|jQuery} [options]  options to apply when creating this tooltip - OR -
     *                                          The markup, or a jquery object, containing the data for the tooltip 
     *                                         
     *  @config {string}        [closeEvents]   A string with one or more comma-separated values that determine when the tooltip
     *                                          closes: 'area-click','tooltip-click','image-mouseout' are valid values
     *                                          then no template will be used.
     *  @config {int}           [offsetx]       the horizontal amount to offset the tooltip 
     *  @config {int}           [offsety]       the vertical amount to offset the tooltip 
     *  @config {string|object} [css]           CSS to apply to the outermost element of the tooltip 
     */
    
    function showToolTip(tooltip,target,image,container,options) {
        var corners,
            ttopts = {};
    
        options = options || {};


        if (target) {

            corners = u.areaCorners(target,image,container,
                                    tooltip.outerWidth(true),
                                    tooltip.outerHeight(true));

            // Try to upper-left align it first, if that doesn't work, change the parameters

            ttopts.left = corners[0];
            ttopts.top = corners[1];

        } else {
            
            ttopts.left = options.left;
            ttopts.top = options.top;
        }

        ttopts.left += (options.offsetx || 0);
        ttopts.top +=(options.offsety || 0);

        ttopts.css= options.css;
        ttopts.fadeDuration = options.fadeDuration;

        showToolTipImpl(tooltip,ttopts);

        return tooltip;
    }
    
    /**
     * Show a tooltip positioned near this area.
      *
     * @param {string|jquery}   [content]       A string of html or a jQuery object containing the tooltip content.
     
     * @param {object|string|jQuery} [options]  options to apply when creating this tooltip - OR -
     *                                          The markup, or a jquery object, containing the data for the tooltip 
     *  @config {string|jquery}   [container]     An element within which the tooltip must be bounded
     *  @config {bool}          [template]      a template to use instead of the default. If this property exists and is null,
     *                                          then no template will be used.
     *  @config {string}        [closeEvents]   A string with one or more comma-separated values that determine when the tooltip
     *                                          closes: 'area-click','tooltip-click','image-mouseout' are valid values
     *                                          then no template will be used.
     *  @config {int}           [offsetx]       the horizontal amount to offset the tooltip 
     *  @config {int}           [offsety]       the vertical amount to offset the tooltip 
     *  @config {string|object} [css]           CSS to apply to the outermost element of the tooltip 
     */
    m.AreaData.prototype.showToolTip= function(content,options) {
        var tooltip, closeOpts, target, tipClosed, template,
            ttopts = {},
            ad=this,
            md=ad.owner,
            areaOpts = ad.effectiveOptions();
    
        // copy the options object so we can update it
        options = options ? $.extend({},options) : {};

        content = content || areaOpts.toolTip;
        closeOpts = options.closeEvents || areaOpts.toolTipClose || md.options.toolTipClose || 'tooltip-click';
        
        template = typeof options.template !== 'undefined' ? 
                options.template :
                md.options.toolTipContainer;

        options.closeEvents = typeof closeOpts === 'string' ?
            closeOpts = u.split(closeOpts) :
            closeOpts;

        options.fadeDuration = options.fadeDuration ||
                 (md.options.toolTipFade ? 
                    (md.options.fadeDuration || areaOpts.fadeDuration) : 0);

        target = ad.area ? 
            ad.area :
            $.map(ad.areas(),
                function(e) {
                    return e.area;
                });

        if (md.activeToolTipID===ad.areaId) {
            return;
        }

        md.clearToolTip();

        md.activeToolTip = tooltip = createToolTip(content,
            template,
            options.css);

        md.activeToolTipID = ad.areaId;

        tipClosed = function() {
            md.clearToolTip();
        };

        bindToolTipClose(closeOpts,'area-click', 'click', $(md.map), null, tipClosed);
        bindToolTipClose(closeOpts,'tooltip-click', 'click', tooltip,null, tipClosed);
        bindToolTipClose(closeOpts,'image-mouseout', 'mouseout', $(md.image), function(e) {
            return (e.relatedTarget && e.relatedTarget.nodeName!=='AREA' && e.relatedTarget!==ad.area);
        }, tipClosed);


        showToolTip(tooltip,
                    target,
                    md.image,
                    options.container,
                    template,
                    options);

        u.ifFunction(md.options.onShowToolTip, ad.area,
        {
            toolTip: tooltip,
            options: ttopts,
            areaOptions: areaOpts,
            key: ad.key,
            selected: ad.isSelected()
        });

        return tooltip;
    };
    

    /**
     * Parse an object that could be a string, a jquery object, or an object with a "contents" property
     * containing html or a jQuery object.
     * 
     * @param  {object|string|jQuery} options The parameter to parse
     * @return {string|jquery} A string or jquery object
     */
    function getHtmlFromOptions(options) {

            // see if any html was passed as either the options object itself, or the content property

            return (options ?
                ((typeof options === 'string' || options.jquery) ?
                    options :
                    options.content) :
                null);
    }

    /**
     * Activate or remove a tooltip for an area. When this method is called on an area, the
     * key parameter doesn't apply and "options" is the first parameter.
     *
     * When called with no parameters, or "key" is a falsy value, any active tooltip is cleared.
     * 
     * When only a key is provided, the default tooltip for the area is used. 
     * 
     * When html is provided, this is used instead of the default tooltip.
     * 
     * When "noTemplate" is true, the default tooltip template will not be used either, meaning only
     * the actual html passed will be used.
     *  
     * @param  {string|AreaElement} key The area for which to activate a tooltip, or a DOM element.
     * 
     * @param {object|string|jquery} [options] options to apply when creating this tooltip - OR -
     *                                         The markup, or a jquery object, containing the data for the tooltip 
     *  @config {string|jQuery} [content]   the inner content of the tooltip; the tooltip text or HTML
     *  @config {Element|jQuery} [container]   the inner content of the tooltip; the tooltip text or HTML
     *  @config {bool}          [template]  a template to use instead of the default. If this property exists and is null,
     *                                      then no template will be used.
     *  @config {int}           [offsetx]   the horizontal amount to offset the tooltip.
     *  @config {int}           [offsety]   the vertical amount to offset the tooltip.
     *  @config {string|object} [css]       CSS to apply to the outermost element of the tooltip 
     *  @config {string|object} [css] CSS to apply to the outermost element of the tooltip 
     *  @config {bool}          [fadeDuration] When non-zero, the duration in milliseconds of a fade-in effect for the tooltip.
     * @return {jQuery} The jQuery object
     */
    
    m.impl.tooltip = function (key,options) {
        return (new m.Method(this,
        function mapData() {
            var tooltip, target, md=this;
            if (!key) {
                md.clearToolTip();
            } else {
                target=$(key);
                if (md.activeToolTipID ===target[0]) {
                    return;
                }
                md.clearToolTip();

                md.activeToolTip = tooltip = createToolTip(getHtmlFromOptions(options),
                            options.template || md.options.toolTipContainer,
                            options.css);
                md.activeToolTipID = target[0];

                bindToolTipClose(['tooltip-click'],'tooltip-click', 'click', tooltip, null, function() {
                    md.clearToolTip();
                });

                md.activeToolTip = tooltip = showToolTip(tooltip,
                    target,
                    md.image,
                    options.container,
                    options);
            }
        },
        function areaData() {
            if ($.isPlainObject(key) && !options) {
                options = key;
            }

            this.showToolTip(getHtmlFromOptions(options),options);
        },
        { 
            name: 'tooltip',
            args: arguments,
            key: key
        }
    )).go();
    };
	
	$('#hall-seat-plan').mapster({
		mapKey: 'data-seat',
        fillColor: 'ace0aa',
        fillOpacity: .7,
		singleSelect: true,
		toolTipClose: ["area-mouseout"],
		areas: [{
			key: "sold",
				staticState: true,
				highlight: false,
				fillColor: 'aac4e8',
				fillOpacity: .7,
			},
			{
			key: "a1",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A1</strong></p><span>Selected</span>')
				
			},
			{
			key: "a2",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A2</strong></p><span>Selected</span>'),
			},
			{
			key: "a3",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A3</strong></p><span>Selected</span>'),
			},
			{
			key: "a4",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A4</strong></p><span>Selected</span>'),
			},
			{
			key: "a5",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A5</strong></p><span>Selected</span>'),
			},
			{
			key: "a6",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A6</strong></p><span>Selected</span>'),
			},
			{
			key: "b1",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B1</strong></p><span>Selected</span>'),
			},
			{
			key: "b2",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B2</strong></p><span>Selected</span>'),
			},
			{
			key: "b3",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B3</strong></p><span>Selected</span>'),
			},
			{
			key: "b4",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B4</strong></p><span>Selected</span>'),
			},
			{
			key: "b5",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B5</strong></p><span>Selected</span>'),
			},
			{
			key: "b6",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B6</strong></p><span>Selected</span>'),
			},
			{
			key: "c1",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C1</strong></p><span>Selected</span>'),
			},
			{
			key: "c2",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C2</strong></p><span>Selected</span>'),
			},
			{
			key: "c3",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C3</strong></p><span>Selected</span>'),
			},
			{
			key: "c4",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C4</strong></p><span>Selected</span>'),
			},
			{
			key: "c5",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C5</strong></p><span>Selected</span>'),
			},
			{
			key: "c6",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C6</strong></p><span>Selected</span>'),
		}]
	});
	
	$('#stadium-seat-plan').mapster({
		mapKey: 'data-seat',
        fillColor: 'ace0aa',
        fillOpacity: .7,
		singleSelect: true,
		toolTipClose: ["area-mouseout"],
		areas: [{
			key: "sold",
				staticState: true,
				highlight: false,
				fillColor: 'aac4e8',
				fillOpacity: .7,
			},
			{
			key: "a1",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A1</strong></p><span>Selected</span>')
				
			},
			{
			key: "a2",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A2</strong></p><span>Selected</span>'),
			},
			{
			key: "a3",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A3</strong></p><span>Selected</span>'),
			},
			{
			key: "a4",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A4</strong></p><span>Selected</span>'),
			},
			{
			key: "a5",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A5</strong></p><span>Selected</span>'),
			},
			{
			key: "a6",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A6</strong></p><span>Selected</span>'),
			},
			{
			key: "a7",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A7</strong></p><span>Selected</span>'),
			},
			{
			key: "a8",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A8</strong></p><span>Selected</span>'),
			},
			{
			key: "a9",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A9</strong></p><span>Selected</span>'),
			},
			{
			key: "a10",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A10</strong></p><span>Selected</span>'),
			},
			{
			key: "a11",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A11</strong></p><span>Selected</span>'),
			},
			{
			key: "a12",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A12</strong></p><span>Selected</span>'),
			},
			{
			key: "a13",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A13</strong></p><span>Selected</span>'),
			},
			{
			key: "a14",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A14</strong></p><span>Selected</span>'),
			},
			{
			key: "a15",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A15</strong></p><span>Selected</span>'),
			},
			{
			key: "a16",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>A16</strong></p><span>Selected</span>'),
			},
			{
			key: "b1",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B1</strong></p><span>Selected</span>'),
			},
			{
			key: "b2",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B2</strong></p><span>Selected</span>'),
			},
			{
			key: "b3",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B3</strong></p><span>Selected</span>'),
			},
			{
			key: "b4",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B4</strong></p><span>Selected</span>'),
			},
			{
			key: "b5",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B5</strong></p><span>Selected</span>'),
			},
			{
			key: "b6",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B6</strong></p><span>Selected</span>'),
			},
			{
			key: "b7",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B7</strong></p><span>Selected</span>'),
			},
			{
			key: "b8",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B8</strong></p><span>Selected</span>'),
			},
			{
			key: "b9",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B9</strong></p><span>Selected</span>'),
			},
			{
			key: "b10",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B10</strong></p><span>Selected</span>'),
			},
			{
			key: "b11",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B11</strong></p><span>Selected</span>'),
			},
			{
			key: "b12",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B12</strong></p><span>Selected</span>'),
			},
			{
			key: "b13",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B13</strong></p><span>Selected</span>'),
			},
			{
			key: "b14",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B14</strong></p><span>Selected</span>'),
			},
			{
			key: "b15",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B15</strong></p><span>Selected</span>'),
			},
			{
			key: "b16",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>B16</strong></p><span>Selected</span>'),
			},
			{
			key: "c1",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C1</strong></p><span>Selected</span>'),
			},
			{
			key: "c2",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C2</strong></p><span>Selected</span>'),
			},
			{
			key: "c3",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C3</strong></p><span>Selected</span>'),
			},
			{
			key: "c4",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C4</strong></p><span>Selected</span>'),
			},
			{
			key: "c5",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C5</strong></p><span>Selected</span>'),
			},
			{
			key: "c6",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C6</strong></p><span>Selected</span>'),
			},
			{
			key: "c7",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C7</strong></p><span>Selected</span>'),
			},
			{
			key: "c8",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C8</strong></p><span>Selected</span>'),
			},
			{
			key: "c9",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C9</strong></p><span>Selected</span>'),
			},
			{
			key: "c10",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C10</strong></p><span>Selected</span>'),
			},
			{
			key: "c11",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C11</strong></p><span>Selected</span>'),
			},
			{
			key: "c12",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C12</strong></p><span>Selected</span>'),
			},
			{
			key: "c13",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C13</strong></p><span>Selected</span>'),
			},
			{
			key: "c14",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C14</strong></p><span>Selected</span>'),
			},
			{
			key: "c15",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C15</strong></p><span>Selected</span>'),
			},
			{
			key: "c16",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>C16</strong></p><span>Selected</span>'),
			},
			{
			key: "d1",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D1</strong></p><span>Selected</span>'),
			},
			{
			key: "d2",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D2</strong></p><span>Selected</span>'),
			},
			{
			key: "d3",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D3</strong></p><span>Selected</span>'),
			},
			{
			key: "d4",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D4</strong></p><span>Selected</span>'),
			},
			{
			key: "d5",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D5</strong></p><span>Selected</span>'),
			},
			{
			key: "d6",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D6</strong></p><span>Selected</span>'),
			},
			{
			key: "d7",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D7</strong></p><span>Selected</span>'),
			},
			{
			key: "d8",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D8</strong></p><span>Selected</span>'),
			},
			{
			key: "d9",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D9</strong></p><span>Selected</span>'),
			},
			{
			key: "d10",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D10</strong></p><span>Selected</span>'),
			},
			{
			key: "d11",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D11</strong></p><span>Selected</span>'),
			},
			{
			key: "d12",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D12</strong></p><span>Selected</span>'),
			},
			{
			key: "d13",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D13</strong></p><span>Selected</span>'),
			},
			{
			key: "d14",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D14</strong></p><span>Selected</span>'),
			},
			{
			key: "d15",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D15</strong></p><span>Selected</span>'),
			},
			{
			key: "d16",
				toolTip: $('<img src="images/seat-preview.jpg"><p>Section <strong>D16</strong></p><span>Selected</span>'),
		}]
	});
	
	
	
} (jQuery));