import React, { Component } from 'react';

export default class SearchForm extends Component {
    constructor(props) {
        super(props);
    }

    handleGroupIdChange = e => this.props.onFilterChange(e.target.name, e.target.value)

    handleIsLocalChange = e => this.props.onFilterChange(e.target.name, e.target.value)

    handleEvaluatedStatusIdChange = e => this.props.onFilterChange(e.target.name, e.target.value)

    handlePAStatusIdChange = e => this.props.onFilterChange(e.target.name, e.target.value)

    handleSubmit = e => {
        e.preventDefault();
        this.props.onSubmit();
    }

    render() {
        return (
            <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                <div className="form-group" >
                    <div className="col">
                        <label className="control-label size-l">Team</label>
                        <div className="control-content">
                            <select value={this.props.filter.groupId} name="groupId" className="form-control" onChange={this.handleGroupIdChange}>
                                <option value="">-- All --</option>
                                {this.props.groups.map(e => <option key={e.value} value={e.value}>{e.label}</option>)}
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <label className="control-label">Category</label>
                        <div className="control-content">
                            <select value={this.props.filter.isLocal} name="isLocal" className="form-control" onChange={this.handleIsLocalChange}>
                                <option value="">-- All --</option>
                                <option value="true">Local</option>
                                <option value="false">Expat</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <label className="control-label size-l">Evaluated Status</label>
                        <div className="control-content">
                            <select value={this.props.filter.evaluatedStatusId} name="evaluatedStatusId" className="form-control" onChange={this.handleEvaluatedStatusIdChange}>
                                <option value="">-- All --</option>
                                <option value="true">Evaluated</option>
                                <option value="false">Not Evaluated</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <label className="control-label">PA Status</label>
                        <div className="control-content">
                            <select value={this.props.filter.paStatusId} name="paStatusId" className="form-control" onChange={this.handlePAStatusIdChange}>
                                <option value="">-- All --</option>performanceStatuses
                                {this.props.performanceStatuses.map(e => <option key={e.id} value={e.id}>{e.name}</option>)}
                            </select>
                        </div>
                    </div>
                    <button type="submit" className="btn btn-primary">Search</button>
                </div>
            </form>
        );
    }
}