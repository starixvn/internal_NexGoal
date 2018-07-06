import React, { Component } from 'react';
import ReactDOM from 'react-dom';

import commonFunctions from '../../common-functions';
import AppLayout from '../../AppLayout';
import Filter from './Filter';
import Table from './Table';
import SubmitForm from './SubmitForm';

function Breadcrumb() {
    return (
        <div className="col-sm-12 sub-header">
            <ul className="row breadcrumb">
                <li className="current">Role Management</li>
            </ul>
        </div>
    );
}

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            optionsState: null,
            options: [],
            functions: [],
            isHost: false
        };
    }

    componentWillMount() {
        this.getAssignPermissions(null);
    }

    getAssignPermissions(roleId) {
        $.ajax({
            type: 'GET',
            url: commonFunctions.getRootUrl() + 'Role/AssignPermission',
            data: {
                roleId: roleId
            },
            success: function (response) {
                this.setState({
                    optionsState: response.roleId,
                    options: response.roles,
                    functions: response.functions,
                    isHost: response.isHost
                });
            }.bind(this),
            error: commonFunctions.handleAjaxError
        });
    }

    handleRoleChange(roleId) {
        this.getAssignPermissions(roleId);
    }

    handleCheckboxChange(func) {
        let functionId = func.functionId;
        let parentId = func.parentId;
        let policy = func.policy;
        let functions = this.state.functions.slice();

        if (parentId) {
            for (let i = 0; i < functions.length; i++) {
                if (functions[i].functionId === parentId) {
                    functions[i].policy = policy;
                }
            }
        } else {
            for (let i = 0; i < functions.length; i++) {
                if (functions[i].parentId === functionId) {
                    functions[i].policy = policy;
                }
            }
        }

        this.setState({
            optionsState: this.state.optionsState,
            options: this.state.options.slice(),
            functions: functions,
            isHost: this.state.isHost
        });
    }

    handleSubmit(event) {
        event.preventDefault();

        let roleId = this.state.optionsState;
        let permissions = this.state.functions.slice().map(func => {
            return { roleId: roleId, functionId: func.functionId, policy: func.policy };
        });

        $.ajax({
            type: 'POST',
            url: commonFunctions.getRootUrl() + 'Role/AssignPermission',
            data: {
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                roleId: roleId,
                permissions: permissions
            },
            success: function (response) {
                if (response.isSuccessful) {
                    $('.msg-success').removeClass('hidden');

                    setTimeout(function () {
                        $('.msg-success').addClass('hidden');
                    }, 3000);
                } else {
                    $('.msg-error').text(response.Message).removeClass('hidden');

                    setTimeout(function () {
                        $('.msg-error').text('').addClass('hidden');
                    }, 3000);
                }
            }.bind(this),
            error: commonFunctions.handleAjaxError
        });
    }

    render() {
        return (
            <AppLayout>
                <div className="row">
                    <Breadcrumb />
                    <div className="col-sm-12">
                        <div className="msg-success -has-border hidden">
                            {document.getElementById('UpdatePermissionSuccessfully').value}
                        </div>
                        <div className="-has-border msg-error hidden"></div>

                        <Filter
                            options={this.state.options}
                            optionsState={this.state.optionsState}
                            handleRoleChange={this.handleRoleChange.bind(this)} />

                        <Table
                            functions={this.state.functions}
                            isHost={this.state.isHost}
                            handleCheckboxChange={this.handleCheckboxChange.bind(this)} />

                        <SubmitForm isHost={this.state.isHost}
                            handleSubmit={this.handleSubmit.bind(this)} />
                    </div>
                </div>
            </AppLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('rolePage'));