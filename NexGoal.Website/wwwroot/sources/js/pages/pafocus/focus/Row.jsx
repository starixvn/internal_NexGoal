import React, { Component, Fragment } from 'react';

import ObjectiveModal from './ObjectiveModal';

export default class Row extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowObjectiveModal: false,
            objectiveSelected: {
                name: '',
                value: ''
            }
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (this.props.data !== nextProps.data
            || this.props.focusEvaluationLevelOptions !== nextProps.focusEvaluationLevelOptions
            || this.state.isShowObjectiveModal !== nextState.isShowObjectiveModal
            || this.state.objectiveSelected !== nextState.objectiveSelected) {
            return true;
        }

        return false;
    }

    getCssClass = () => {
        let pafocus = this.props.data;
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
        let index = this.props.focusEvaluationLevelOptions.findIndex(item => item.value.toString() === value);

        if (index >= 0) {
            return this.props.focusEvaluationLevelOptions[index].name;
        }

        return '-';
    }

    renderContribution = () => {
        let pafocus = this.props.data;

        return (
            <Fragment>
                {
                    pafocus.isHost &&
                    <select className="form-control" value={pafocus.contribution} name="contribution"
                        onChange={this.handleOnChange}
                    >
                        {
                            !pafocus.contribution &&
                            <option />
                        }
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

    renderBonusGiven = () => {
        let pafocus = this.props.data;

        return (
            <Fragment>
                {
                    pafocus.isHost &&
                    <select className="form-control" value={pafocus.bonusGiven} name="bonusGiven"
                        onChange={this.handleOnChange}
                    >
                        {
                            !pafocus.bonusGiven &&
                            <option />
                        }
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

    renderReadyPromotion = () => {
        let pafocus = this.props.data;
        let cssClass = pafocus.isReadyPromotion === null || pafocus.isReadyPromotion === '' ? '' : pafocus.isReadyPromotion === 'true' ? 'good' : 'normal';
        let label = pafocus.isReadyPromotion === null || pafocus.isReadyPromotion === '' ? '-' : pafocus.isReadyPromotion === 'true' ? 'Yes' : 'No';

        return (
            <Fragment>
                {
                    pafocus.isHost &&
                    <select className="form-control" value={pafocus.isReadyPromotion} name="isReadyPromotion"
                        onChange={this.handleOnChange}
                    >
                        {
                            !pafocus.isReadyPromotion &&
                            <option />
                        }
                        <option value="true">Yes</option>
                        <option value="false">No</option>
                    </select>
                }
                {
                    !pafocus.isHost &&
                    <span className={`focus-evaluation-status ${cssClass}`}>
                        {label}
                    </span>
                }
            </Fragment>
        );
    }

    renderContentObjective = (objective) => {
        return (
            <Fragment>
                {
                    objective !== null && objective.length > 0 &&
                    <Fragment>
                        {objective.length > 50 ? `${objective.substring(0, 50)} ...` : objective}
                    </Fragment>
                }
            </Fragment>
        );
    }

    renderActionObjective = (propName) => {
        let pafocus = this.props.data;

        return (
            <Fragment>
                {
                    pafocus.isHost &&
                    <a className="icon-square-edit-outline" title="Edit" name={propName} onClick={this.handleShowObjectiveModal} />
                }
                {
                    !pafocus.isHost &&
                    <Fragment>
                        {
                            pafocus[propName] !== null && pafocus[propName].length > 0 &&
                            <Fragment>
                                {
                                    pafocus[propName].length > 25 &&
                                    <a className="icon-comment-eye-outline" title="View" name={propName} onClick={this.handleShowObjectiveModal} />
                                }
                            </Fragment>
                        }
                    </Fragment>
                }
            </Fragment>
        );
    }

    renderGrade = (originalGrade, grade) => {
        let gradeId = grade ? grade.gradeId : 0;
        let originalGradeId = originalGrade ? originalGrade.gradeId : 0;

        let gradeClass = '';
        if (gradeId > originalGradeId) {
            gradeClass = 'icon-increase-arrow';
        }

        if (gradeId < originalGradeId) {
            gradeClass = 'icon-descrease-arrow';
        }

        return <span>{grade ? grade.name : 'N/A'}<span className={gradeClass} /></span>;
    }

    handleOnChange = (event) => {
        let pafocus = this.props.data;

        this.props.onChange(pafocus.employee.groupCode, pafocus.paFocusId, event.target.name, event.target.value);
    }

    handleChangeObjective = (objective) => {
        let pafocus = this.props.data;

        this.props.onChange(pafocus.employee.groupCode, pafocus.paFocusId, objective.name, objective.value);
    }

    handleShowObjectiveModal = (event) => {
        let targetName = event.target.name;
        let value = this.props.data[targetName] ? this.props.data[targetName] : '';

        let objective = {
            name: targetName,
            value: value
        };

        this.setState({
            isShowObjectiveModal: true,
            objectiveSelected: objective
        });
    }

    handleCloseObjectiveModal = () => {
        this.setState({ isShowObjectiveModal: false });
    }

    render() {
        let pafocus = this.props.data;

        return (
            <Fragment>
                <tr className={this.getCssClass()}>
                    <td className="text-center">{this.props.rowIndex}</td>
                    <td>{pafocus.employee.nickName}</td>
                    <td>{pafocus.employee.fullName}</td>
                    <td>{pafocus.employee.jobTitle}</td>
                    <td className="text-center">
                        {this.renderContribution()}
                    </td>
                    <td className="text-center">
                        {this.renderBonusGiven()}
                    </td>
                    <td className="text-center">
                        {this.renderReadyPromotion()}
                    </td>
                    <td className="text-center">
                        {this.renderGrade(pafocus.originalGrade, pafocus.grade)}
                    </td>
                    <td>
                        {this.renderContentObjective(pafocus.currentObjective)}
                    </td>
                    <td className="text-center width-40">
                        {this.renderActionObjective('currentObjective')}
                    </td>
                    <td>
                        {this.renderContentObjective(pafocus.nextObjective)}
                    </td>
                    <td className="text-center width-40">
                        {this.renderActionObjective('nextObjective')}
                    </td>
                </tr>
                {
                    this.state.isShowObjectiveModal &&
                    <ObjectiveModal
                        isHost={pafocus.isHost}
                        objective={this.state.objectiveSelected}
                        isShow={this.state.isShowObjectiveModal}
                        onClose={this.handleCloseObjectiveModal}
                        onSubmit={this.handleChangeObjective}
                    />
                }
            </Fragment>
        );
    }
}