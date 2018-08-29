import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import ToastLayout from '../../shared/layouts/ToastLayout';
import commonFunctions from '../../common-functions';
import Breadcrumb from '../../shared/components/Breadcrumb';
import { Policy } from '../../constants';
import Filter from './Filter';
import Table from './Table';
import SubmitForm from './SubmitForm';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            selectedOption: null,
            options: [],
            functions: [],
            isHost: false
        };

        this.handleOnRoleChange = this.handleOnRoleChange.bind(this);
        this.handleOnCheckboxChange = this.handleOnCheckboxChange.bind(this);
        this.handleOnSubmit = this.handleOnSubmit.bind(this);
    }

    componentDidMount() {
        this.getAssignPermissions(null);
    }

    getAssignPermissions(roleId) {
        axios.get(commonFunctions.getRootUrl() + `Role/AssignPermission?roleId=${roleId}`)
            .then(response => {
                let data = response.data;

                this.setState({
                    selectedOption: data.roleId,
                    options: data.roles,
                    functions: data.functions,
                    isHost: data.isHost
                });
            }).catch(commonFunctions.axiosError);
    }

    handleOnRoleChange(roleId) {
        this.getAssignPermissions(roleId);
    }

    handleOnCheckboxChange(functionObj, target, isChecked) {
        let functionId = functionObj.functionId;
        let parentId = functionObj.parentId;
        let functions = this.state.functions.slice();

        let index = functions.indexOf(functionObj);
        if (isChecked) {
            functions[index].policy = target === 'funcRead' ? Policy.Read : Policy.Host;
        } else {
            functions[index].policy = target === 'funcRead' ? Policy.None : functions[index].policy === Policy.Host ? Policy.Read : Policy.None;
        }

        if (parentId) {
            let childs = functions.filter(f => f.parentId === parentId);
            let policy = Math.max.apply(Math, childs.map(f => { return f.policy; }));

            for (index = 0; index < functions.length; index++) {
                if (functions[index].functionId === parentId) {
                    functions[index].policy = policy;
                    break;
                }
            }
        } else {
            for (index = 0; index < functions.length; index++) {
                if (functions[index].parentId === functionId) {
                    if (isChecked) {
                        functions[index].policy = target === 'funcRead' ? Policy.Read : Policy.Host;
                    } else {
                        functions[index].policy = target === 'funcRead' ?
                            Policy.None : functions[index].policy === Policy.Host ? Policy.Read : Policy.None;
                    }
                }
            }
        }

        this.setState({ functions: functions });
    }

    handleOnSubmit(event) {
        event.preventDefault();

        let roleId = this.state.selectedOption;
        let permissions = this.state.functions.slice().map(func => {
            return { roleId: roleId, functionId: func.functionId, policy: func.policy };
        });

        axios({
            url: commonFunctions.getRootUrl() + 'Role/AssignPermission',
            method: 'POST',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                roleId: roleId,
                permissions: permissions
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
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
                            selectedOption={this.state.selectedOption}
                            onRoleChange={this.handleOnRoleChange}
                        />

                        <Table
                            functions={this.state.functions}
                            isHost={this.state.isHost}
                            onCheckboxChange={this.handleOnCheckboxChange}
                        />

                        <SubmitForm isHost={this.state.isHost}
                            onSubmit={this.handleOnSubmit}
                        />
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('role-wrapper'));