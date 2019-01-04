import React, { Component, Fragment } from 'react';

import getResource from '../../resourceHelper';
import { NominationStatuses } from '../../constants';
import { getNominationClassName, noDataIndication } from '../../common-functions';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    renderEmployee = (employee) =>
        <tr key={employee.employeeId}>
            <td>{employee.nickName}</td>
            <td></td>
        </tr>

    renderSetting = (data) => {
        return (
            <Fragment key={data.pm.employeeId}>
                <tr >
                    <td>{data.pm.nickName} <a class="fa fa-plus" title="Add"></a></td>
                    <td className='text-center'><a class="fa fa-trash-o remove" title="Delete"></a></td>
                </tr>
                {data.employees.map(this.renderEmployee)}
            </Fragment>

        );
    }

    render() {
        let content = this.props.tableData.length > 0 ? this.props.tableData.map(this.renderSetting)
            : <tr><td className="text-center" colSpan={2}>{noDataIndication()}</td></tr>;

        return (
            <table className="table table-bordered table-striped tbl-nomination-report">
                <thead>
                    <tr>
                        <th className="order">{getResource('ProjectManager')}</th>
                        <th className="nominee"></th>
                    </tr>
                </thead>
                <tbody>
                    {content}
                </tbody>
            </table>
        );
    }
}