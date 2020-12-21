import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { toast } from 'react-toastify';
import { stringify } from 'qs';

import ToastLayout from '../../shared/layouts/ToastLayout';
import { axiosError, resolveClientUrl } from '../../common-functions';
import { Policy, PostRequestHeaders } from '../../constants';
import getResource from '../../resourceHelper';
import Breadcrumb from '../../shared/components/Breadcrumb';
import SearchForm from './SearchForm';
import Table from './table/Table';
import SubmitForm from './SubmitForm';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            roleSelected: null,
            options: [],
            functions: [],
            isHost: false
        };
    }

    componentDidMount() {
        this.getAssignPermissions(null);
    }

    getAssignPermissions = roleId => {
        axios.get(resolveClientUrl(`Role/AssignPermission?roleId=${roleId}`))
            .then(response => {
                const data = response.data;

                this.setState({
                    roleSelected: data.roleId,
                    options: data.roles,
                    functions: data.functions,
                    isHost: data.isHost
                });
            }).catch(axiosError);
    };

    handleOnRoleChange = roleId => {
        this.getAssignPermissions(roleId);
    };

    setFunctionPolicy = (func, target, isChecked) => {
        if (isChecked) {
            func.policy = target === 'funcRead' ? Policy.Read : Policy.Host;
            return;
        }

        if (target === 'funcRead') {
            func.policy = Policy.None;
            return;
        }

        if (func.policy === Policy.Host) {
            func.policy = func.onlyPolicy === Policy.Host ? Policy.None : Policy.Read;
        } else {
            func.policy = Policy.None;
        }
    };

    handleOnCheckboxChange = (functionObj, target, isChecked) => {
        const { functionId, parentId } = functionObj;
        const functions = this.state.functions.slice();
        const totalItems = functions.length;

        let index = functions.indexOf(functionObj);
        this.setFunctionPolicy(functions[index], target, isChecked);

        if (parentId) {
            const childs = functions.filter(f => f.parentId === parentId);
            const policy = Math.max.apply(Math, childs.map(f => f.policy));

            for (index = 0; index < totalItems; index++) {
                if (functions[index].functionId === parentId) {
                    functions[index].policy = policy;
                    break;
                }
            }
        } else {
            for (index = 0; index < totalItems; index++) {
                if (functions[index].parentId === functionId) {
                    this.setFunctionPolicy(functions[index], target, isChecked);
                }
            }
        }

        this.setState({ functions: functions });
    };

    handleOnSubmit = event => {
        event.preventDefault();

        const roleId = this.state.roleSelected;
        const permissions = this.state.functions.slice().map(func => {
            return { roleId: roleId, functionId: func.functionId, policy: func.policy };
        });

        axios({
            url: resolveClientUrl('Role/AssignPermission'),
            method: 'POST',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                roleId: roleId,
                permissions: permissions
            }),
            headers: PostRequestHeaders
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatePermissionSuccessfully'));
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    };

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">{getResource('RolePermission')}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <SearchForm roleOptions={this.state.options}
                            roleSelected={this.state.roleSelected}
                            onRoleChange={this.handleOnRoleChange}
                        />

                        <Table functions={this.state.functions}
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