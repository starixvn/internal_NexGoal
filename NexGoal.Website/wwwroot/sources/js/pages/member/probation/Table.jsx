import React, { Component } from 'react';

import getResource from '../../../resourceHelper';
import { noDataIndication } from '../../../common-functions';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    renderRow = probationer => (
        <tr key={probationer.employeeId}>
            <td>{probationer.nickName}</td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td>
                {probationer.canEdit &&
                    <a className="fa fa-pencil" title="Edit" data-employeeid={probationer.employeeId} onClick={this.props.onEdit} />
                }
            </td>
        </tr>
    )

    render() {
        if (this.props.tableData) {
            return (
                <table className="table table-bordered" >
                    <thead>
                        <tr>
                            <th rowSpan="2">{getResource('NickName')}</th>
                            <th colSpan="2">{getResource('Self')}</th>
                            <th colSpan="3">{getResource('Evaluator')}</th>
                            <th colSpan="2">{getResource('Reviewer')}</th>
                            <th rowSpan="2">{getResource('Revised')}</th>
                            <th rowSpan="2">{getResource('Action')}</th>

                        </tr>
                        <tr>
                            <th>{getResource('Score')}</th>
                            <th>{getResource('Comment')}</th>
                            <th>{getResource('Score')}</th>
                            <th>{getResource('Result')}</th>
                            <th>{getResource('Comment')}</th>
                            <th>{getResource('FinalResult')}</th>
                            <th>{getResource('Comment')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.props.tableData.length > 0 ? this.props.tableData.map(this.renderRow)
                            : <tr><td className="text-center" colSpan={10}>{noDataIndication()}</td></tr>}
                    </tbody>
                </table>
            );
        }

        return '';
    }
}