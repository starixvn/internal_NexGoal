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
            <form className="form-inline form-filter" onSubmit={this.handleSubmit}>
                <div className="form-group" >
                    <div className="col">
                        <label className="control-label">Type</label>
                        <div className="control-content">
                            <select value={this.props.typeId} name="typeId" className="form-control" onChange={this.handleTypeIdChange}>
                                <option value="">-- All --</option>
                                <option value="true">Active</option>
                                <option value="false">Inactive</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <label className="control-label">Status</label>
                        <div className="control-content">
                            <select value={this.props.isActive} name="isActive" className="form-control" onChange={this.handleStatusChange}>
                                <option value="">-- All --</option>
                                <option value="true">Probation</option>
                                <option value="false">Semi-anual</option>
                            </select>
                        </div>
                    </div>
                    <div className="col">
                        <button type="submit" className="btn btn-primary">Search</button>
                    </div>
                </div>
            </form>
        );
    }
}