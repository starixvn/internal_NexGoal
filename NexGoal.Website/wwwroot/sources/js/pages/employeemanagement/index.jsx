import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
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
import SearchForm from './SearchForm';
import EditEmployeeRole from './edit-employee-role';

const { SearchBar } = Search;

export default class EmployeeManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                keyword: '',
                group: { value: '', label: '-- All --' },
                isDisabled: false
            },
            groupOptions: [],
            data: [],
            showModal: false,
            isHost: document.getElementById('IsHost').checked
        };
    }

    componentDidMount() {
        this.getGroups();
        this.getEmployees();
    }

    employeeSelected = {};

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    handleFilterSubmit = () => {
        this.getEmployees();
    }

    getGroups = () => {
        axios.get(resolveClientUrl('Common/GetGroups'))
            .then(response => {
                let groupOptions = response.data.map((group) => {
                    return { value: group.groupId, label: group.name };
                });
                groupOptions.unshift({ value: '', label: '-- All --' });

                this.setState({ groupOptions: groupOptions });
            }).catch(axiosError);
    }

    getEmployees = () => {
        const filter = this.state.filter;
        axios.get(resolveClientUrl('employee/GetEmployees'), {
            params: {
                keyword: filter.keyword,
                groupId: filter.group.value,
                isDisabled: filter.isDisabled
            }
        }).then(response => {
            this.setState({ data: response.data });
        }).catch(axiosError);
    }

    getEmployeeById = (id, callback) => {
        axios.get(resolveClientUrl(`employee/GetEmployeeById?employeeId=${id}`))
            .then(response => {
                if (typeof (callback) === 'function')
                    callback(response.data);
            }).catch(axiosError);
    }

    editEmployee = (employee) => {
        this.employeeSelected = employee;
        this.showModal();
    }

    closeModal = () => {
        this.setState({ showModal: false });
    }

    showModal = () => {
        this.setState({ showModal: true });
    }

    handleEditEmployeeRolesSuccess = (data) => {
        if (data.isSuccessful) {
            toast.success(getResource('UpdatedSuccessfully'));

            this.getEmployeeById(this.employeeSelected.employeeId, (updatedEmployee) => {
                var data = this.state.data.slice();
                var index = data.findIndex((e) => e.employeeId === this.employeeSelected.employeeId);
                data[index] = { ...data[index], ...updatedEmployee };

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
                isDummyField: true,
                text: `${getResource('SymbolCount')}`,
                classes: 'text-center',
                headerStyle: { width: '50px' },
                formatter: (cell, row, rowIndex) => {
                    return rowIndex + 1;
                }
            },
            {
                dataField: 'code',
                text: `${getResource('Code')}`,
                classes: 'text-center',
                headerStyle: { width: '80px' }
            },
            {
                dataField: 'nickName',
                text: `${getResource('NickName')}`,
                headerStyle: { width: '120px' }
            },
            {
                dataField: 'fullName',
                text: `${getResource('FullName')}`
            },
            {
                dataField: 'groupName',
                text: `${getResource('Team')}`
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
                text: `${getResource('EditRole')}`,
                classes: 'text-center',
                headerStyle: { width: '80px' },
                isDummyField: true,
                formatter: (cell, row) => {
                    if (this.state.isHost) {
                        return (
                            <a className="fa fa-pencil-square-o" onClick={this.editEmployee.bind(this, row)} />
                        );
                    }

                    return '';
                }
            }];

        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">{getResource('EmployeeManagement')}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <SearchForm filter={this.state.filter}
                            groupOptions={this.state.groupOptions}
                            onFilterChange={this.handleFilterChange}
                            onFilterSubmit={this.handleFilterSubmit}
                        />

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
                        {
                            this.state.showModal &&
                            <EditEmployeeRole showModal={this.state.showModal}
                                closeModal={this.closeModal}
                                employee={this.employeeSelected}
                                callback={this.handleEditEmployeeRolesSuccess}
                            />
                        }
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<EmployeeManagement />, document.getElementById("employee-wrapper"));