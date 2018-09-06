import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { Form, FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import { toast } from 'react-toastify';
import axios from 'axios';

import { PaginationOptions } from '../../constants';
import { axiosError, noDataIndication, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import EditUserRole from './edit-user-role';

const { SearchBar } = Search;

export default class UserManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                keyword: '',
                groupId: '',
                isDisabledUser: false
            },
            groups: [],
            data: [],
            showModal: false,
            isHost: document.getElementById('IsHost').checked
        };
    }

    componentDidMount = () => {
        this.getUsers();
        this.getGroups();
    }

    userRoles = [];
    currentEditUser = [];

    onFilterChange = (e) => {
        var filter = { [e.target.name]: e.target.value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    onIsDisabledUserChange = (e) => {
        var filter = { [e.target.name]: e.target.checked };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    handleSubmit = (event) => {
        event.preventDefault();
        this.getUsers();
    }

    getGroups = () => {
        axios.get(resolveClientUrl('Common/GetGroups'))
            .then(response => {
                this.setState({ groups: response.data });
            }).catch(axiosError);
    }

    getUsers = () => {
        const filter = this.state.filter;
        axios.get(resolveClientUrl(`User/GetUsers?keyword=${filter.keyword}&groupId=${filter.groupId}&isDisabledUser=${filter.isDisabledUser}`))
            .then(response => {
                response.data.map((e, index) => e.rowNum = index + 1);
                this.setState({ data: response.data });
            }).catch(axiosError);
    }

    getUser = (id, callback) => {
        axios.get(resolveClientUrl(`User/GetUser?employeeId=${id}`))
            .then(response => {
                if (typeof (callback) === 'function')
                    callback(response.data);
            }).catch(axiosError);
    }

    editUser = (user) => {
        this.currentEditUser = user;
        this.showModal();
    }

    closeModal = () => {
        this.setState({ showModal: false });
    }

    showModal = () => {
        this.setState({ showModal: true });
    }

    handleEditUserRolesSuccess = (data) => {
        if (data.isSuccessful) {
            toast.success(getResource('UpdatedSuccessfully'));

            this.getUser(this.currentEditUser.employeeId, (updatedUser) => {
                var data = this.state.data.slice();
                var index = data.findIndex((e) => e.employeeId === this.currentEditUser.employeeId);
                data[index] = { ...data[index], ...updatedUser };

                this.setState({ data: data, showModal: false });
            });
        } else {
            toast.error(data.message);
        }
    }

    render() {
        const columns = [
            {
                dataField: 'rowNum',
                text: '#',
                classes: 'text-center'
            },
            {
                dataField: 'code',
                text: `${getResource('Code')}`,
                classes: 'text-center'
            },
            {
                dataField: 'nickName',
                text: `${getResource('NickName')}`
            },
            {
                dataField: 'fullName',
                text: `${getResource('FullName')}`
            },
            {
                dataField: 'groupCode',
                text: `${getResource('Team')}`,
                classes: 'text-center'
            },
            {
                dataField: 'jobTitle',
                text: `${getResource('JobTitle')}`
            },
            {
                dataField: 'roles',
                text: `${getResource('Roles')}`
            },
            {
                dataField: '',
                text: `${getResource('Action')}`,
                classes: 'text-center',
                formatter: (cell, row, rowIndex, colIndex) => {
                    if (this.state.isHost) {
                        return (
                            <a className="fa fa-pencil-square-o" title="Assign role to user" onClick={this.editUser.bind(this, row)} />
                        );
                    }

                    return '';
                }
            }];

        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">{getResource('UserManagement')}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Form inline className="form-filter" onSubmit={this.handleSubmit}>
                            <FormGroup>
                                <ControlLabel>{getResource('Keyword')}</ControlLabel>
                                <FormControl name="keyword" type="text"
                                    placeholder="Full name or Nick name" autoComplete="off"
                                    onChange={this.onFilterChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel>{getResource('Team')}</ControlLabel>
                                <FormControl componentClass="select" onChange={this.onFilterChange} name="groupId">
                                    <option value="">-- All --</option>
                                    {this.state.groups.map((group) => <option key={group.groupId} value={group.groupId}>{group.name}</option>)}
                                </FormControl>
                            </FormGroup>
                            <FormGroup>
                                <label>
                                    <input type="checkbox" name="isDisabledUser" onChange={this.onIsDisabledUserChange} /> Is Disabled User
                                </label>
                            </FormGroup>
                            <FormGroup>
                                <Button type="submit" bsStyle="primary">{getResource('Search')}</Button>
                            </FormGroup>
                        </Form>

                        <ToolkitProvider keyField="employeeId" data={this.state.data} columns={columns} search >
                            {
                                props => {
                                    return (
                                        <Fragment>
                                            <div className="search-wrapper clearfix" >
                                                <div className="search pull-right">
                                                    <SearchBar {...props.searchProps} placeholder="" delay={100} className="search-textbox" />
                                                    <span className="fa fa-search icon" />
                                                </div>
                                            </div>
                                            <BootstrapTable striped {...props.baseProps}
                                                pagination={paginationFactory(PaginationOptions)}
                                                noDataIndication={noDataIndication}
                                            />
                                        </Fragment>
                                    );
                                }
                            }
                        </ToolkitProvider>
                    </div>
                    <div>
                        {this.state.showModal &&
                            <EditUserRole showModal={this.state.showModal}
                                closeModal={this.closeModal}
                                user={this.currentEditUser}
                                callback={this.handleEditUserRolesSuccess}
                            />
                        }
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<UserManagement />, document.getElementById("user-wrapper"));