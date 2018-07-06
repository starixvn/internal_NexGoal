import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import { Modal, Button, Form, FormGroup, ControlLabel, Checkbox, Col, FormControl } from 'react-bootstrap';

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
            tableData: [],
            showModal: false,
            currentPage: 1,
            sizePerPage: 20
        };

        this.tableRef = React.createRef();
        this.searchRef = React.createRef();
    }

    userRoles = [];
    currentEditUser = [];
    resetPage = false;

    componentDidMount = function () {
        this.getUsers();
        this.getGroups();
    }

    onFilterChange = (e) => {
        var filter = { [e.target.name]: e.target.value };
        this.setState({ filter: { ...this.state.filter, ...filter } });
    }

    onTableFilterChange = (e) => {
        this.resetPage = true;
        var filterData = this.getFilteredTableData(e.target.value, this.state.data);
        this.setState({ tableData: filterData });
    }

    getFilteredTableData = (searchValue, data) => {
        if (searchValue && searchValue.length > 1) {
            var value = searchValue.trim().toLowerCase();
            var filterDatas = data.filter((employee) =>
                employee.nickName.toLowerCase().indexOf(value) >= 0
                || employee.fullName.toLowerCase().indexOf(value) >= 0
                || employee.code.toString().indexOf(value) >= 0
                || employee.group.toLowerCase().indexOf(value) >= 0
                || employee.jobTitle.toLowerCase().indexOf(value) >= 0
                || (employee.roles && employee.roles.toLowerCase().indexOf(value) >= 0)
            );
            return filterDatas;
        } else {
            return data;
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
                var filterData = this.getFilteredTableData(this.searchRef.current.value, data);
                this.setState({ data: data, tableData: filterData });
            });
    }

    editUser = (user) => {
        this.currentEditUser = user;
        fetch(`/UserManagement/GetRolesForUser?employeeId=${user.employeeId}`,
            {
                credentials: "same-origin"
            })
            .then((response) => response.json())
            .then(data => {
                this.userRoles = data;
                this.showModal();
            });
    }

    closeModal = () => {
        this.setState({ showModal: false });
    }

    showModal = () => {
        this.setState({ showModal: true });
    }

    handleEditUserRoles = (e) => {
        e.preventDefault();
        var selectedIds = [];
        Array.from(e.target.querySelectorAll('input')).map(e => {
            if (e.checked)
                selectedIds.push(e.value);
        });
        var employeeId = this.currentEditUser.employeeId;
        fetch('/UserManagement/EditUserRoles',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
                },
                credentials: "same-origin",
                body: `employeeId=${employeeId}&selectedRoleIds=${selectedIds.toString()}&__RequestVerificationToken=${document.getElementsByName('__RequestVerificationToken')[0].value}`
            })
            .then((response) => {
                this.closeModal();
                this.getUsers();
            });
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
                        <a className="fa fa-pencil-square-o" title="Assign role to user" onClick={this.editUser.bind(this, row)}></a>
                    );
                }
            }];

        const paginationOptions = paginationFactory({
            page: this.tableRef.current && !this.resetPage ? this.tableRef.current.store._page : 1,
            sizePerPage: this.tableRef.current ? this.tableRef.current.store._sizePerPage : 20,
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
        this.resetPage = false;

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
                        <input className="search-textbox" ref={this.searchRef} onChange={this.onTableFilterChange} />
                        <span className="fa fa-search icon"></span>
                    </div>
                    {this.state.tableData.length > 0 &&
                        <BootstrapTable ref={this.tableRef} keyField='employeeId' data={this.state.tableData} columns={columns} pagination={paginationOptions} />
                    }
                </div>
                <div>
                    <Modal show={this.state.showModal} onHide={this.closeModal}>
                        <Modal.Header closeButton>
                            <Modal.Title>Assign Roles To User </Modal.Title>
                        </Modal.Header>
                        <Form id="edit-user-role-form" horizontal onSubmit={this.handleEditUserRoles}>
                            <Modal.Body>
                                <FormGroup>
                                    <Col componentClass={ControlLabel} sm={2}>Nick Name</Col>
                                    <div className="col-sm-10">
                                        <ControlLabel>{this.currentEditUser.nickName}</ControlLabel>
                                    </div>
                                </FormGroup>
                                <FormGroup>
                                    <Col componentClass={ControlLabel} sm={2}>
                                        Roles
                                    </Col>
                                    <Col sm={10}>
                                        {this.userRoles.map(role =>
                                            <Checkbox key={role.roleId} defaultChecked={role.hasRole} value={role.roleId}>{role.role}</Checkbox>
                                        )}
                                    </Col>
                                </FormGroup>
                            </Modal.Body>
                            <Modal.Footer>
                                <Button onClick={this.closeModal}>Cancel</Button>
                                <Button type="submit" className="btn btn-primary">Submit</Button>
                            </Modal.Footer>
                        </Form>
                    </Modal>
                </div>
            </div>
        );
    }
}

ReactDOM.render(<UserManagement />, document.getElementById("user-management-wrapper"));