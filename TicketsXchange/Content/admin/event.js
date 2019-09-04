﻿$(function () {
    // crud table
    altair_crud_table.init();
});

altair_crud_table = {
    init: function () {

        $('#event_table').jtable({
            paging: true, //Enable paging
            pageSize: 10, //Set page size (default: 10)
            addRecordButton: $('#recordAdd'),
            deleteConfirmation: function (data) {
                data.deleteConfirmMessage = 'Are you sure to delete user ' + data.record.Name + '?';
            },
            formCreated: function (event, data) {
                // replace click event on some clickable elements
                // to make icheck label works
                data.form.find('.jtable-option-text-clickable').each(function () {
                    var $thisTarget = $(this).prev().attr('id');
                    $(this)
                        .attr('data-click-target', $thisTarget)
                        .off('click')
                        .on('click', function (e) {
                            e.preventDefault();
                            $('#' + $(this).attr('data-click-target')).iCheck('toggle');
                        })
                });
                // create selectize
                data.form.find('select').each(function () {
                    var $this = $(this);
                    $this.after('<div class="selectize_fix"></div>')
                        .selectize({
                            dropdownParent: 'body',
                            placeholder: 'Click here to select ...',
                            onDropdownOpen: function ($dropdown) {
                                $dropdown
                                    .hide()
                                    .velocity('slideDown', {
                                        begin: function () {
                                            $dropdown.css({ 'margin-top': '0' })
                                        },
                                        duration: 200,
                                        easing: easing_swiftOut
                                    })
                            },
                            onDropdownClose: function ($dropdown) {
                                $dropdown
                                    .show()
                                    .velocity('slideUp', {
                                        complete: function () {
                                            $dropdown.css({ 'margin-top': '' })
                                        },
                                        duration: 200,
                                        easing: easing_swiftOut
                                    })
                            }
                        });
                });
                // create icheck
                data.form
                    .find('input[type="checkbox"],input[type="radio"]')
                    .each(function () {
                        var $this = $(this);
                        $this.iCheck({
                            checkboxClass: 'icheckbox_md',
                            radioClass: 'iradio_md',
                            increaseArea: '20%'
                        })
                            .on('ifChecked', function (event) {
                                $this.val(1);
                            })
                            .on('ifUnchecked', function (event) {
                                $this.val(0);
                            })
                    });
                // reinitialize inputs
                data.form.find('.jtable-input').children('input[type="text"],input[type="password"],textarea').not('.md-input').each(function () {
                    $(this).addClass('md-input');
                    altair_forms.textarea_autosize();
                });
                altair_md.inputs();
            },
            actions: {
                listAction: '/api/Event/Admin?action=list',
                createAction: '/api/Event/Admin?action=create',
                updateAction: '/api/Event/Admin?action=update',
                deleteAction: '/api/Event/Admin?action=delete'
            },
            fields: {
                Id: {
                    key: true,
                    create: false,
                    edit: false,
                    list: false
                },
                CategoryId: {
                    title: 'Category',
                    width: '7%',
                    options: '/api/Category/GetJson'
                },
                Name: {
                    title: 'Name',
                    width: '15%'
                },
                Details: {
                    title: 'Details',
                    width: '20%'
                },
                CreatedAt: {
                    title: 'Created Date',
                    width: '5%',
                    create: false,
                    edit: false
                }
            }
        }).jtable('load');

        // change buttons visual style in ui-dialog
        $('.ui-dialog-buttonset')
            .children('button')
            .attr('class', '')
            .addClass('md-btn md-btn-flat')
            .off('mouseenter focus');
        $('#AddRecordDialogSaveButton,#EditDialogSaveButton,#DeleteDialogButton').addClass('md-btn-flat-primary');

    }
};