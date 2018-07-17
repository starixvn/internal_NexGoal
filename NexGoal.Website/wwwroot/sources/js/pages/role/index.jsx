import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { toast } from 'react-toastify';

import ToastLayout from '../../shared/layouts/ToastLayout';
import commonFunctions from '../../common-functions';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Filter from './Filter';
import Table from './Table';
import SubmitForm from './SubmitForm';

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
        axios.get(commonFunctions.getRootUrl() + `Role/AssignPermission?roleId=${roleId}`)
            .then(response => {
                let data = response.data;
                this.setState({
                    optionsState: data.roleId,
                    options: data.roles,
                    functions: data.functions,
                    isHost: data.isHost
                });
            }).catch(commonFunctions.axiosError);
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
            let childs = functions.filter(f => f.parentId === parentId);
            policy = Math.max.apply(Math, childs.map(f => { return f.policy; }));

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

        this.setState({ functions: functions });
    }

    handleSubmit(event) {
        event.preventDefault();

        let roleId = this.state.optionsState;
        let permissions = this.state.functions.slice().map(func => {
            return { roleId: roleId, functionId: func.functionId, policy: func.policy };
        });
        let formData = new FormData();
        formData.append('__RequestVerificationToken', document.getElementsByName('__RequestVerificationToken')[0].value);
        formData.append('roleId', roleId);
        formData.append('permissions', JSON.stringify(permissions));

        axios({
            url: commonFunctions.getRootUrl() + 'Role/AssignPermission',
            method: 'POST',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success(document.getElementById('UpdatePermissionSuccessfully').value);
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Role Management</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
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
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('role-wrapper'));