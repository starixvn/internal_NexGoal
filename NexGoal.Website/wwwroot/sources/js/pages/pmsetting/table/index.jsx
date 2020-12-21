import React, { Component } from 'react';

import getResource from '../../../resourceHelper';
import { noDataIndication } from '../../../common-functions';
import Row from './Row';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    renderSetting = data => <Row key={data.pm.employeeId} data={data} onAddMember={this.props.onAddMember} onRemoveMember={this.props.onRemoveMember} />;

    render() {
        const content = this.props.tableData.length > 0 ? this.props.tableData.map(this.renderSetting)
            : <tr><td className="text-center" colSpan={3}>{noDataIndication()}</td></tr>;

        return (
            <table className="table table-bordered table-striped tbl-pm-assignment">
                <thead>
                    <tr>
                        <th>{getResource('Employee')}</th>
                        <th>{getResource('JobTitle')}</th>
                        <th className="text-center col-120">{getResource('Remove')}</th>
                    </tr>
                </thead>
                <tbody>
                    {content}
                </tbody>
            </table>
        );
    }
}