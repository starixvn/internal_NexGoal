import React, { PureComponent } from 'react';

import getResource from '../../../resourceHelper';
import EvaluatorCol from './EvaluatorCol';

export default class FlowSettingTable extends PureComponent {
    render() {
        const { isHost, isSubmit, isGroup, evaluator, reviewer, employeeOptions } = this.props;

        return (
            <table className="table table-bordered">
                <thead>
                    <tr>
                        <th className="member">{isGroup ? getResource('Team') : getResource('Member')}</th>
                        <th className="evaluator">{getResource('Evaluator')}</th>
                        <th className="reviewer">{getResource('Reviewer')}</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        {this.props.selfColContent}
                        <td className="col-evaluator">
                            <EvaluatorCol isHost={isHost} evaluator={evaluator}
                                isSubmit={isSubmit} options={employeeOptions}
                                onChange={this.props.onChangeEvaluator}
                            />
                        </td>
                        <td className="col-reviewer">
                            <EvaluatorCol isHost={isHost} evaluator={reviewer}
                                isSubmit={isSubmit} options={employeeOptions}
                                onChange={this.props.onChangeReviewer}
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
        );
    }
}