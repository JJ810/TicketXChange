$(function () {
    // crud table
    altair_crud_table.init();
});

altair_crud_table = {
    init: function () {

        $('#user_table').jtable({
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
                listAction: '/api/User/Admin?action=list',
                createAction: '/api/User/Admin?action=create',
                updateAction: '/api/User/Admin?action=update',
                deleteAction: '/api/User/Admin?action=delete'
            },
            fields: {
                Id: {
                    key: true,
                    create: false,
                    edit: false,
                    list: false
                },
                FirstName: {
                    title: 'First Name',
                    width: '7%'
                },
                LastName: {
                    title: 'Last Name',
                    width: '7%'
                },
                Email: {
                    title: 'Email address',
                    width: '8%'
                },
                Sex: {
                    title: 'Gender',
                    width: '5%',
                    options: { 0: 'Male', 1: 'Female' }
                },
                DOB: {
                    title: 'Birth Date',
                    width: '10%'
                },
                Verified: {
                    title: 'Email Verified',
                    width: '4%',
                    type: 'checkbox',
                    values: { '0': '', '1': '<span class="uk-badge uk-badge-success">Verified</span>' },
                    defaultValue: '0'
                },
                Active: {
                    title: 'Active',
                    width: '4%',
                    type: 'checkbox',
                    values: { '0': '', '1': '<span class="uk-badge uk-badge-success">Active</span>' },
                    defaultValue: '0'
                },
                MobileNumber: {
                    title: 'Mobile Number',
                    width: '7%'
                },
                PhoneNumber: {
                    title: 'Phone Number',
                    width: '7%'
                },
                AddressLine1: {
                    title: 'Address Line1',
                    width: '7%'
                },
                AddressLine2: {
                    title: 'Address Line2',
                    width: '7%'
                },
                AddressLine3: {
                    title: 'Address Line3',
                    width: '7%'
                },
                PostCode: {
                    title: 'Post Code',
                    width: '7%'
                },
                City: {
                    title: 'City',
                    width: '7%'
                },
                State: {
                    title: 'State',
                    width: '7%'
                },
                Country: {
                    title: 'Country',
                    width: '7%'
                },
                CreatedAt: {
                    title: 'Record date',
                    width: '10%',
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