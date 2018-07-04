import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import { debug } from 'util';

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
            tableData: []
        };

        this.getUsers();
        this.getGroups();
    }

    onFilterChange = (e) => {
        var filter = { [e.target.name]: e.target.value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    onTableFilterChange = (e) => {
        if (e.target.value && e.target.value.length > 1) {
            var value = e.target.value.trim().toLowerCase();
            var filterDatas = this.state.data.filter((employee) =>
                employee.nickName.toLowerCase().indexOf(value) >= 0
                || employee.fullName.toLowerCase().indexOf(value) >= 0
                || employee.code.toString().indexOf(value) >= 0
                || employee.group.toLowerCase().indexOf(value) >= 0
                || employee.jobTitle.toLowerCase().indexOf(value) >= 0
                || (employee.roles && employee.roles.toLowerCase().indexOf(value) >= 0)
            );
            this.setState({ tableData: filterDatas });
        } else {
            this.setState({ tableData: this.state.data });
        }
    }

    handleSubmit = (event) => {
        event.preventDefault();
        this.getUsers();
    }

    getGroups = () => {
        fetch('/UserManagement/GetGroups',
            {
                credentials: "same-origin"
            })
            .then((response) => response.json())
            .then(data => {
                this.setState({ groups: data });
            });
    }

    getUsers = () => {
        fetch(`/UserManagement/GetUsers?keyword=${this.state.filter.keyword}&groupId=${this.state.filter.groupId}&isLocal=${this.state.filter.isLocal}&isDisabledUser=${this.state.filter.isDisabledUser}`,
            {
                credentials: "same-origin"
            })
            .then((response) => response.json())
            .then(data => {
                this.setState({ data: data, tableData: data });
            });
    }

    editUser = (userId) => {
        alert(userId);
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
                    return (
                        <a className="fa fa-pencil-square-o" title="Assign role to user" onClick={() => this.editUser(row.employeeId)}></a>
                    );
                }
            }];

        const paginationOptions = paginationFactory({
            sizePerPage: 20,
            showTotal: false,
            sizePerPageList: [
                {
                    text: '5', value: 5
                },
                {
                    text: '10', value: 10
                },
                {
                    text: '15', value: 15
                },
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

        return (
            <div className="row">
                <div className="col-sm-12 sub-header">
                    <ul className="row breadcrumb">
                        <li className="current">User Management</li>
                    </ul>
                </div>
                <div className="col-sm-12">
                    <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                        <div className="form-group" >
                            <div className="col" >
                                <label className="control-label" data-translate="Keyword">Keyword</label>
                                <div className="control-content">
                                    <input name="keyword" className="form-control" type="text" placeholder="Full name or Nick name" onChange={this.onFilterChange} />
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label ng-scope" data-translate="Team">Team</label>
                                <div className="control-content">
                                    <select name="groupId" className="form-control" onChange={this.onFilterChange}>
                                        <option value="">-- All --</option>
                                        {this.state.groups.map((group) => <option key={group.groupId} value={group.groupId}>{group.name}</option>)}
                                    </select>
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label" data-translate="Category">Category</label>
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
                    <div className="search-wrap pull-right" >
                        <input className="search-textbox" onChange={this.onTableFilterChange} />
                        <span class="fa fa-search icon"></span>
                    </div>
                    {this.state.data.length > 0 &&
                        <BootstrapTable keyField='employeeId' data={this.state.tableData} columns={columns} pagination={paginationOptions} />
                    }
                </div>
            </div>
        );
    }
}

ReactDOM.render(<UserManagement />, document.getElementById("main-container"));