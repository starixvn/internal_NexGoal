import React, { Component, Fragment } from 'react';

import getResource from '../../../resourceHelper';

export default class Row extends Component {
    getCssClass = (pafocus) => {
        let className = '';
        let contribution = pafocus.contribution ? parseInt(pafocus.contribution) : 0;
        let bonusGiven = pafocus.bonusGiven ? parseInt(pafocus.bonusGiven) : 0;
        let isReadyPromotion = pafocus.isReadyPromotion ? pafocus.isReadyPromotion === 'true' : false;

        if (contribution + bonusGiven > 8) {
            className = `noticeable-performance ${isReadyPromotion ? 'outstanding' : 'very-good'}`;
        } else if (contribution > 0 && bonusGiven > 0 && contribution + bonusGiven < 4) {
            className = 'noticeable-performance poor';
        }

        return className;
    }

    getFocusEvaluationLevel = (value) => {
        let index = this.props.focusEvaluationLevelOptions.map(function (item) { return item.Value; }).indexOf(value);

        if (index >= 0) {
            return this.props.focusEvaluationLevelOptions[index].name;
        }

        return '-';
    }

    renderContributionCell = (pafocus) => {
        return (
            <Fragment>
                {
                    pafocus.isHost &&
                    <select className="form-control" value={pafocus.contribution} name="contribution"
                        onChange={this.handleOnChange}
                    >
                        <option />
                        {
                            this.props.focusEvaluationLevelOptions.map(option => {
                                return (
                                    <option key={`contribution${option.value}`} value={option.value}>{option.name}</option>
                                );
                            })
                        }
                    </select>
                }
                {
                    !pafocus.isHost &&
                    this.getFocusEvaluationLevel(pafocus.contribution)
                }
            </Fragment>
        );
    }

    renderBonusGivenCell = (pafocus) => {
        return (
            <Fragment>
                {
                    pafocus.isHost &&
                    <select className="form-control" value={pafocus.bonusGiven} name="bonusGiven"
                        onChange={this.handleOnChange}
                    >
                        <option />
                        {
                            this.props.focusEvaluationLevelOptions.map(option => {
                                return (
                                    <option key={`bonusGiven${option.value}`} value={option.value}>{option.name}</option>
                                );
                            })
                        }
                    </select>
                }
                {
                    !pafocus.isHost &&
                    this.getFocusEvaluationLevel(pafocus.bonusGiven)
                }
            </Fragment>
        );
    }

    renderReadyPromotionCell = (pafocus) => {
        return (
            <Fragment>
                {
                    pafocus.isHost &&
                    <select className="form-control" value={pafocus.isReadyPromotion} name="isReadyPromotion"
                        onChange={this.handleOnChange}
                    >
                        <option />
                        <option value="true">Yes</option>
                        <option value="false">No</option>
                    </select>
                }
                {
                    !pafocus.isHost &&
                    <span className={pafocus.isReadyPromotion ? 'good' : 'normal'}>
                        {pafocus.isReadyPromotion === null ? '-' : pafocus.isReadyPromotion ? 'Yes' : 'No'}
                    </span>
                }
            </Fragment>
        );
    }

    renderCurrentObjectiveCell = (pafocus) => {
        return (
            <Fragment>
                {
                    (pafocus.currentObjective === null || pafocus.currentObjective.length === 0) &&
                    <Fragment>-</Fragment>
                }
                {
                    pafocus.currentObjective !== null && pafocus.currentObjective.length > 0 &&
                    <Fragment>
                        {pafocus.currentObjective.length > 25 ? `${pafocus.currentObjective.substring(0, 25)} ...` : pafocus.currentObjective}
                    </Fragment>
                }
            </Fragment>
        );
    }

    renderNextObjectiveCell = (pafocus) => {
        return (
            <Fragment>
                {
                    (pafocus.nextObjective === null || pafocus.nextObjective.length === 0) &&
                    <Fragment>-</Fragment>
                }
                {
                    pafocus.nextObjective !== null && pafocus.nextObjective.length > 0 &&
                    <Fragment>
                        {pafocus.nextObjective.length > 25 ? `${pafocus.nextObjective.substring(0, 25)} ...` : pafocus.nextObjective}
                    </Fragment>
                }
            </Fragment>
        );
    }

    handleOnChange = (event) => {
        const pafocus = this.props.data;

        this.props.onChange(pafocus.employee.groupCode, pafocus.paFocusId, event.target.name, event.target.value);
    }

    render() {
        const pafocus = this.props.data;

        return (
            <tr className={this.getCssClass(pafocus)}>
                <td className="text-center">{this.props.rowIndex}</td>
                <td>{pafocus.employee.nickName}</td>
                <td>{pafocus.employee.fullName}</td>
                <td>{pafocus.employee.jobTitle}</td>
                <td className="text-center">
                    {this.renderContributionCell(pafocus)}
                </td>
                <td className="text-center">
                    {this.renderBonusGivenCell(pafocus)}
                </td>
                <td className="text-center">
                    {this.renderReadyPromotionCell(pafocus)}
                </td>
                <td className="text-center">
                    {this.renderCurrentObjectiveCell(pafocus)}
                </td>
                <td className="text-center">
                    {this.renderNextObjectiveCell(pafocus)}
                </td>
            </tr>
        );
    }
}