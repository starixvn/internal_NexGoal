import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import BootstrapTable from 'react-bootstrap-table-next';
import Breadcrumb from '../../shared/components/Breadcrumb';
import Notification from '../../shared/components/notification';

export default class DisciplineManagement extends Component {
    constructor(props) {
        super(props);

        this.tableRef = React.createRef();
        this.searchRef = React.createRef();
        this.notificationRef = React.createRef();
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">Incident Management</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Notification ref={this.notificationRef} />
                    <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                        <div className="form-group" >
                            <div className="col" >
                                <label className="control-label">From</label>
                                <div className="control-content">
                                    <input name="from" className="form-control" type="text" onChange={this.onFilterChange} />
                                </div>
                            </div>
                            <div className="col" >
                                <label className="control-label">From</label>
                                <div className="control-content">
                                    <input name="to" className="form-control" type="text" onChange={this.onFilterChange} />
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label">Status</label>
                                <div className="control-content">
                                    <select name="isActive" className="form-control" onChange={this.onFilterChange}>
                                        <option value="">-- All --</option>
                                        <option value="true">Active</option>
                                        <option value="false">Inactive</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div className="form-group">
                            <div className="col">
                                <label className="control-label">Team</label>
                                <div className="control-content">
                                    <select className="form-control" name="groupIds" onChange={this.onFilterChange}>
                                        <option value="">-- All --</option>
                                        <option value="false">Expat</option>
                                        <option value="true">Local</option>
                                    </select>
                                </div>
                            </div>
                            <div className="col">
                                <label className="control-label">Employee</label>
                                <div className="control-content">
                                    <input className="form-control" type="text" name="employeeId" placeholder="-- Search an employee --" />
                                </div>
                            </div>
                            <div className="col">
                                <button className="btn btn-primary">Search</button>
                            </div>
                        </div>
                    </form>
                    <div className="search-wrapper clearfix" >
                        <div className="pull-left">
                            <button className="btn btn-primary"><i className="fa fa-plus"></i> Add Incident Record</button>
                        </div>
                        <div className="search pull-right">
                            <input className="search-textbox" ref={this.searchRef} onChange={this.onTableFilterChange} />
                            <span className="fa fa-search icon"></span>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

ReactDOM.render(<DisciplineManagement />, document.getElementById("discipline-wrapper"));