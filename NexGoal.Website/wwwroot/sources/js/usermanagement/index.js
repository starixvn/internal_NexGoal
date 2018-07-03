import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';

export default class UserManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                keyword: '',
                groupId: '',
                categoryId: '',
                isDisabledUser: false
            },
            data: []
        };

        this.getUsers();
    }

    onFilterChange = (e) => {
        this.setState({ filter: { [e.target.name]: e.target.value } });
    }

    handleSubmit(event) {
        event.preventDefault();
        this.getUsers();
    }

    getUsers() {
        fetch(`/UserManagement/GetUsers?keyword=${this.state.filter.keyword}&groupId=${this.state.filter.groupId}&categoryId=${this.state.filter.categoryId}&isDisabledUser=${this.state.filter.isDisabledUser}`,
            {
                credentials: "same-origin"
            })
            .then((response) => response.json())
            .then(data => {
                this.setState({ data: data });
            });
    }

    render() {
        const columns = [
            {
                dataField: 'userId',
                text: '#'
            },
            {
                dataField: 'code',
                text: 'Code'
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
                text: 'Team'
            },
            {
                dataField: 'jobTitle',
                text: 'Job Title'
            },
            {
                dataField: 'roles',
                text: 'Roles'
            }];

        const paginationOptions = paginationFactory({
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
                                        <option value="" className="ng-binding">-- All --</option>
                                        <option value="0" label="Finance &amp; Accounting">Finance &amp; Accounting</option>
                                        <option value="1" label="IT Network">IT Network</option>
                                        <option value="2" label="Rome">Rome</option>
                                        <option value="3" label="Olympus">Olympus</option>
                                        <option value="4" label="DeepBlue">DeepBlue</option>
                                        <option value="5" label="Design">Design</option>
                                        <option value="6" label="Terra">Terra</option>
                                        <option value="7" label="Nexus">Nexus</option>
                                        <option value="8" label="Management">Management</option>
                                        <option value="9" label="Project Management">Project Management</option>
                                        <option value="10" label="HR-Admin">HR-Admin</option>
                                        <option value="11" label="Expat">Expat</option>
                                        <option value="12" label="Sun">Sun</option>
                                        <option value="13" label="Web Team Leader">Web Team Leader</option>
                                        <option value="14" label="Business Analysis">Business Analysis</option>
                                        <option value="15" label="Pontus">Pontus</option>
                                        <option value="16" label="Training and Documentation">Training and Documentation</option>
                                        <option value="17" label="Research and Development">Research and Development</option>
                                        <option value="18" label="DEV Pool">DEV Pool</option>
                                        <option value="19" label="Iris">Iris</option>
                                        <option value="20" label="QC Team Leader">QC Team Leader</option>
                                        <option value="21" label="Special Project Unit 1">Special Project Unit 1</option>
                                        <option value="22" label="Sparta">Sparta</option>
                                        <option value="23" label="Everest">Everest</option><option value="24" label="Starix Application Support">Starix Application Support</option>
                                        <option value="25" label="Database Administration">Database Administration</option>
                                        <option value="26" label="Standby">Standby</option>
                                        <option value="27" label="Special Project Unit 2">Special Project Unit 2</option>
                                        <option value="28" label="NexOps">NexOps</option>
                                        <option value="29" label="Project Support">Project Support</option>
                                    </select>
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label" data-translate="Category">Category</label>
                                <div className="control-content">
                                    <select className="form-control" name="categoryId" onChange={this.onFilterChange}>
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
                    <BootstrapTable keyField='userId' data={this.state.data} columns={columns} pagination={paginationOptions} />
                </div>
            </div>
        );
    }
}

ReactDOM.render(<UserManagement />, document.getElementById("main-container"));