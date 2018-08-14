import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import ToolkitProvider, { Search } from 'react-bootstrap-table2-toolkit';
import { toast } from 'react-toastify';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import EditUserRole from './edit-user-role';

const { SearchBar } = Search;

const paginationOptions = paginationFactory({
    page: 1,
    sizePerPage: 20,
    showTotal: false,
    sizePerPageList: [
        {
            text: '20', value: 20
        },
        {
            text: '50', value: 50
        },
        {
            text: '100', value: 100
        },
        {
            text: '200', value: 200
        }
    ]
});

export default class UserManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                keyword: '',
                groupId: '',
                isLocal: '',
                isDisabledUser: false
            },
            groups: [],
            data: [],
            showModal: false,
            currentPage: 1,
            sizePerPage: 20,

            canEditRole: document.getElementById('CanEditUserRoles').value === 'True'
        };

        this.searchRef = React.createRef();
        this.tableRef = React.createRef();
    }

    userRoles = [];
    currentEditUser = [];
    resetPage = false

    componentDidMount = () => {
        this.getUsers();
        this.getGroups();
    }

    onFilterChange = (e) => {
        var filter = { [e.target.name]: e.target.value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    handleSubmit = (event) => {
        event.preventDefault();
        this.getUsers();
    }

    getGroups = () => {
        fetch('/Common/GetGroups',
            {
                credentials: "same-origin"
            })
            .then((response) => response.json())
            .then(data => {
                this.setState({ groups: data });
            });
    }

    getUsers = () => {
        fetch(`/User/GetUsers?keyword=${this.state.filter.keyword}&groupId=${this.state.filter.groupId}&isLocal=${this.state.filter.isLocal}&isDisabledUser=${this.state.filter.isDisabledUser}`,
            {
                credentials: "same-origin"
            })
            .then((response) => response.json())
            .then(data => {
                data.map((e, index) => e.rowNum = index + 1);
                this.setState({ data: data });
            });
    }

    getUser = (id, callback) => {
        fetch(`/User/GetUser?employeeId=${id}`,
            {
                credentials: "same-origin"
            })
            .then((response) => response.json())
            .then(data => {
                if (typeof (callback) === 'function')
                    callback(data);
            });
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
            toast.success('Update successfully!');

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

    noDataDisplay = () => <span className="text-danger">No information available!</span>

    render() {
        const columns = [
            {
                dataField: 'rowNum',
                text: '#',
                classes: 'text-center'
            },
            {
                dataField: 'code',
                text: 'Code',
                classes: 'text-center'
            },
            {
                dataField: 'nickName',
                text: 'Nick Name'
            },
            {
                dataField: 'fullName',
                text: 'Full Name'
            },
            {
                dataField: 'group',
                text: 'Team',
                classes: 'text-center'
            },
            {
                dataField: 'jobTitle',
                text: 'Job Title'
            },
            {
                dataField: 'roles',
                text: 'Roles'
            },
            {
                dataField: '',
                text: 'Action',
                classes: 'text-center',
                formatter: (cell, row, rowIndex, colIndex) => {
                    if (this.state.canEditRole) {
                        return (
                            <a className="fa fa-pencil-square-o" title="Assign role to user" onClick={this.editUser.bind(this, row)}></a>
                        );
                    }

                    return '';
                }
            }];

        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">User Management</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                            <div className="form-group" >
                                <div className="col" >
                                    <label className="control-label">Keyword</label>
                                    <div className="control-content">
                                        <input name="keyword" className="form-control" type="text" placeholder="Full name or Nick name" onChange={this.onFilterChange} />
                                    </div>
                                </div>
                                <div className="col">
                                    <label className="control-label">Team</label>
                                    <div className="control-content">
                                        <select name="groupId" className="form-control" onChange={this.onFilterChange}>
                                            <option value="">-- All --</option>
                                            {this.state.groups.map((group) => <option key={group.groupId} value={group.groupId}>{group.name}</option>)}
                                        </select>
                                    </div>
                                </div>
                                <div className="col">
                                    <label className="control-label">Category</label>
                                    <div className="control-content">
                                        <select className="form-control" name="isLocal" onChange={this.onFilterChange}>
                                            <option value="">-- All --</option>
                                            <option value="false">Expat</option>
                                            <option value="true">Local</option>
                                        </select>
                                    </div>
                                </div>
                                <div className="col">
                                    <label>
                                        <input type="checkbox" name="isDisabledUser" onChange={this.onFilterChange} /> Is Disabled User
                                </label>
                                </div>
                                <div className="col">
                                    <button className="btn btn-primary">Search</button>
                                </div>
                            </div>
                        </form>

                        <ToolkitProvider keyField="employeeId" data={this.state.data} noDataIndication={this.noDataDisplay} columns={columns} search >
                            {
                                props => {
                                    debugger;

                                    return (
                                        <Fragment>
                                            <div className="search-wrapper clearfix" >
                                                <div className="search pull-right">
                                                    <SearchBar {...props.searchProps} placeholder="" delay={500} className="search-textbox" />
                                                    <span className="fa fa-search icon"></span>
                                                </div>
                                            </div>
                                            <BootstrapTable ref={this.tableRef} {...props.baseProps} pagination={paginationOptions} />
                                        </Fragment>
                                    );
                                }
                            }
                        </ToolkitProvider>
                    </div>
                    <div>
                        {this.state.showModal &&
                            <EditUserRole showModal={this.state.showModal} closeModal={this.closeModal} user={this.currentEditUser} callback={this.handleEditUserRolesSuccess} />
                        }
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<UserManagement />, document.getElementById("user-wrapper"));