import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

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
        axios.get(commonFunctions.getRootUrl() + `Role/AssignPermission?roleId=${roleId}`)
            .then(response => {
                let data = response.data;

                this.setState({
                    optionsState: data.roleId,
                    options: data.roles,
                    functions: data.functions,
                    isHost: data.isHost
                });
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
        let permissions = this.state.functions.slice();
        let token = document.getElementsByName('__RequestVerificationToken')[0].value;

        let postData = {
            '__RequestVerificationToken': token,
            'roleId': roleId,
            'permissions': permissions
        };

        let axiosConfig = {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        };

        axios.post(commonFunctions.getRootUrl() + 'Role/AssignPermission', postData, axiosConfig)
            .then(response => {
                console.log(response);
            }).catch(error => {
                console.log(error);
            });
    }

    render() {
        return (
            <AppLayout>
                <div className="row">
                    <Breadcrumb />
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
            </AppLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('rolePage'));