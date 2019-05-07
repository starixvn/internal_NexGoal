import React, { Component, Fragment } from 'react';

import getResource from '../../../resourceHelper';
import { noDataIndication } from '../../../common-functions';
import StickyBlock from '../../../shared/components/StickyBlock';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    renderEmployee = (employee, index) => {
        return (
            <tr key={employee.employeeId}>
                <td className="text-center">{index + 1}</td>
                <td>{employee.nickName} ({employee.fullName})</td>
                <td>{employee.jobTitle}</td>
                <td className="text-center">{employee.selfPRs > 0 ? employee.selfPRs : '-'}</td>
                <td className="text-center">{employee.pmpRs > 0 ? employee.pmpRs : '-'}</td>
                <td className="text-center">{employee.evaluatorPRs > 0 ? employee.evaluatorPRs : '-'}</td>
                <td className="text-center">{employee.reviewerPRs > 0 ? employee.reviewerPRs : '-'}</td>
            </tr>
        );
    }

    renderData = data => {
        return (
            <Fragment key={data.nominationPeriodId}>
                <tr className="team-group-row">
                    <td colSpan={7}>{data.groupName}</td>
                </tr>
                {data.employees.map(this.renderEmployee)}
            </Fragment >
        );
    }

    renderTableContent = () => {
        if (this.props.tableData && this.props.period) {
            return (
                <Fragment>
                    <tr className="group-row">
                        <td colSpan={7}>{this.props.period.name}</td>
                    </tr>
                    {this.props.tableData.map(this.renderData)}
                </Fragment>);
        } else {
            return '';
        }
    }

    render() {
        return (
            <Fragment>
                <StickyBlock threshold={236}>
                    <table className="table table-bordered table-striped pr-tracking-table">
                        <thead>
                            <tr>
                                <th className="order" rowSpan="2">{getResource('SymbolCount')}</th>
                                <th className="nick-name " rowSpan="2">{getResource('Employee')}</th>
                                <th className="job-title" rowSpan="2">{getResource('JobTitle')}</th>
                                <th colSpan="4">Performance Record Quantity</th>
                            </tr>
                            <tr>
                                <th className="self">{getResource('Self')}</th>
                                <th className="pm">{getResource('ProjectManager')}</th>
                                <th className="evaluator">{getResource('Evaluator')}</th>
                                <th className="reviewer">{getResource('Reviewer')}</th>
                            </tr>
                        </thead>
                    </table>
                </StickyBlock>
                <table className="table table-bordered table-striped pr-tracking-table">
                    <thead>
                        <tr>
                            <th className="order" rowSpan="2">{getResource('SymbolCount')}</th>
                            <th className="nick-name " rowSpan="2">{getResource('Employee')}</th>
                            <th className="job-title" rowSpan="2">{getResource('JobTitle')}</th>
                            <th colSpan="4">Performance RecordQuantity</th>
                        </tr>
                        <tr>
                            <th className="self">{getResource('Self')}</th>
                            <th className="pm">{getResource('ProjectManager')}</th>
                            <th className="evaluator">{getResource('Evaluator')}</th>
                            <th className="reviewer">{getResource('Reviewer')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.renderTableContent()}
                    </tbody>
                </table>
            </Fragment>

        );
    }
}