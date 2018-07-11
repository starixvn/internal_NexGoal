import React, { Component } from 'react';

export default class Breadcrumb extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="col-sm-12 sub-header">
                <ul className="row breadcrumb">
                    {this.props.children}
                </ul>
            </div>
        );
    }
}