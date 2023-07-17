import React, { PureComponent } from 'react';

import getResource from '../../../resourceHelper';
import EvaluatorCol from './EvaluatorCol';

export default class FlowSettingTable extends PureComponent {
    render() {
        const { isHost, isSubmit, isGroup, evaluator, reviewer, employeeOptions } = this.props;

        return (
            <div className="flow-settings-detail">
                <div className="setting-item">
                    <div className="header header-member">{isGroup ? getResource('Team') : getResource('Member')}</div>
                    <div className="content"> {this.props.selfColContent}</div>
                </div>
                <div className="setting-item">
                    <div className="header header-evaluator">{getResource('Evaluator')}</div>
                    <div className="content"><EvaluatorCol isHost={isHost} evaluator={evaluator}
                        isSubmit={isSubmit} options={employeeOptions}
                        onChange={this.props.onChangeEvaluator}
                    /></div>
                </div>
                <div className="setting-item">
                    <div className="header header-reviewer">{getResource('Reviewer')}</div>
                    <div className="content"><EvaluatorCol isHost={isHost} evaluator={reviewer}
                        isSubmit={isSubmit} options={employeeOptions}
                        onChange={this.props.onChangeReviewer}
                    /></div>
                </div>
            </div>
        );
    }
}