import React, { PureComponent, Fragment } from 'react';
import { sumBy } from 'lodash';
import moment from 'moment';
import { DateFormat } from '../../constants';

export default class HRM extends PureComponent {
    renderAttendance = (attendace, index) => {
        return (
            <tr key={attendace.month + attendace.year}>
                <td className="text-center">{attendace.month}/{attendace.year}</td>
                <td className="text-center">{attendace.missingCheckInCheckOut}</td>
                <td className="text-center">{attendace.lateIn}</td>
                <td className="text-center">{attendace.earlyOut}</td>
                <td className="text-center">{attendace.unpaidLeave}</td>
                <td className="text-center">{attendace.unplanAnnualLeave}</td>
                <td className="text-center">{attendace.averageWorkingHours}</td>
                <td className="text-center">{attendace.overtime}</td>
                {index === 0 &&
                    <td className="text-center" rowSpan={this.props.hrm.attendances.length + 1}>
                        <div>
                            {this.props.hrm.toeic.toeicScore}
                        </div>
                        {this.props.hrm.toeic.expiredDate &&
                            <div>
                                {moment(this.props.hrm.toeic.expiredDate).format(DateFormat)}
                            </div>
                        }
                    </td>
                }
            </tr>
        );
    }

    render() {
        return (
            <table className="table table-bordered table-striped hrm-table">
                <thead>
                    <tr>
                        <th colSpan="9">A. HRM System</th>
                    </tr>
                    <tr>
                        <th>Month</th>
                        <th>Missing Check In/Out</th>
                        <th>Late In</th>
                        <th>Early Out</th>
                        <th>Unpaid Leave</th>
                        <th>Unpaid Annual Leave</th>
                        <th>Average Working Hours</th>
                        <th>Overtime</th>
                        <th>TOEIC Score</th>
                    </tr>
                </thead>
                <tbody>

                    {this.props.hrm.attendances.map(this.renderAttendance)}

                    <tr>
                        <td className="text-center">Total</td>
                        <td className="text-center">{sumBy(this.props.hrm.attendances, 'missingCheckInCheckOut')}</td>
                        <td className="text-center">{sumBy(this.props.hrm.attendances, 'lateIn')}</td>
                        <td className="text-center">{sumBy(this.props.hrm.attendances, 'earlyOut')}</td>
                        <td className="text-center">{sumBy(this.props.hrm.attendances, 'unpaidLeave')}</td>
                        <td className="text-center">{sumBy(this.props.hrm.attendances, 'unplanAnnualLeave')}</td>
                        <td className="text-center">{sumBy(this.props.hrm.attendances, 'averageWorkingHours')}</td>
                        <td className="text-center">{sumBy(this.props.hrm.attendances, 'overtime')}</td>
                    </tr>
                </tbody>
            </table>
        );
    }
}