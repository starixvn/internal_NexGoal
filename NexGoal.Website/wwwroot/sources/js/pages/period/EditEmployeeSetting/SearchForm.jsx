import React, { Component } from 'react';
import ReactDOM from 'react-dom';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handleTypeIdChange = e => this.props.onTypeIdChange(e.target.value)

    handleStatusChange = e => this.props.onStatusChange(e.target.value)

    render() {
        return (
            <form className="form-inline form-filter" onSubmit={this.props.onSubmit}>
                <div className="form-group" >
                    <div className="col">
                        <label className="control-label size-l">Department/Team</label>
                        <div className="control-content">
                            <select value={this.props.groupId} name="targetTypeId" className="form-control" onChange={this.handleTypeIdChange}>
                                <option value="">-- All --</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <label className="control-label">Category</label>
                        <div className="control-content">
                            <select value={this.props.isLocal} name="isActive" className="form-control" onChange={this.handleStatusChange}>
                                <option value="">-- All --</option>
                                <option value="true">Local</option>
                                <option value="false">Expat</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <label className="control-label size-l">PA Generated Status</label>
                        <div className="control-content">
                            <select value={this.props.isGenerated} name="isActive" className="form-control" onChange={this.handleStatusChange}>
                                <option value="">-- All --</option>
                                <option value="true">Generated</option>
                                <option value="false">Not Generated</option>
                            </select>
                        </div>
                    </div>
                </div>
                <div className="form-group">
                    <div className="col">
                        <label className="control-label size-l">Evaluated/Status</label>
                        <div className="control-content">
                            <select value={this.props.evaluatedStatusId} name="targetTypeId" className="form-control" onChange={this.handleTypeIdChange}>
                                <option value="">-- All --</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <label className="control-label">PA Status</label>
                        <div className="control-content">
                            <select value={this.props.paStatusId} name="targetTypeId" className="form-control" onChange={this.handleTypeIdChange}>
                                <option value="">-- All --</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <div className="control-content">
                            <label>
                                <input type="checkbox" /> Show Excluded Employee
                            </label>
                        </div>
                    </div>
                    <button type="submit" className="btn btn-primary">Search</button>
                </div>
            </form>
        );
    }
}